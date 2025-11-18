using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASMS.Repositories.Data;
using ASMS.Repositories.Entities;
using ASMS.Repositories.Infrastructures;
using ASMS.Services.Interfaces;
using ASMS.Services.Model.CLP;
using Azure.Core;
using Microsoft.Extensions.Logging;

namespace ASMS.Services.Services
{
    public class CLPService : ICLPService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CLPService> _logger;
        private const decimal FLOOR_LENGTH_LIMIT = 1.7m;
        public CLPService(IUnitOfWork unitOfWork, ILogger<CLPService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task<List<ContainerPlacementDto>> FindSuitableContainersAsync(FindContainerRequest request)
        {
            _logger.LogInformation("Finding suitable containers for package: {L}x{W}x{H}, Weight: {Wt}kg, Fragile: {F}",
                request.PackageLength, request.PackageWidth, request.PackageHeight, request.PackageWeight, request.IsFragile);
            //1. Determine container type needed
            var containerTypeId = DetermineContainerType(request.PackageLength, request.PackageWidth, request.PackageHeight);
            if (containerTypeId == 0)
            {
                _logger.LogWarning("Package too large for any container type");
                return new List<ContainerPlacementDto>();
            }

            var containerType = await _unitOfWork.ContainerType.GetByIdAsync(containerTypeId);
            if (containerType == null)
            {
                _logger.LogWarning("Container type {TypeId} not found", containerTypeId);
                return new List<ContainerPlacementDto>();
            }

            // 2. Get available containers of that type
            var availableContainers = await _unitOfWork.Containers.GetAvailableByTypeAsync(containerTypeId);

            if (!availableContainers.Any())
            {
                _logger.LogWarning("No available containers of type {Type}", containerType.Type);
                return new List<ContainerPlacementDto>();
            }
            // 3. Find suitable floors
            var candidates = new List<PlacementCandidate>();

            if (containerTypeId == 4) // Type D - only floor 4
            {
                candidates = await FindPositionsForTypeD(availableContainers, request, containerType);
            }
            else // Type A, B, C - floors 1-3
            {
                candidates = await FindPositionsForTypeABC(availableContainers, request, containerType);
            }
            // 4.Sort by score and return top 10
            var result = candidates
                .OrderByDescending(c => c.Score)
                .Take(10)
                .Select(c => new ContainerPlacementDto
                {
                    ContainerCode = c.Container.ContainerCode,
                    ContainerType = containerType.Type,
                    Length = containerType.Length.GetValueOrDefault(),
                    Width = containerType.Width.GetValueOrDefault(),
                    Height = containerType.Height.GetValueOrDefault(),

                    FloorCode = c.Floor.FloorCode,
                    FloorNumber = c.Floor.FloorNumber.GetValueOrDefault(),
                    ShelfCode = c.Floor.ShelfCode,
                    StorageCode = c.Floor.ShelfCodeNavigation?.StorageCode ?? "",

                    PositionX = c.PositionX,
                    PositionY = c.PositionY,
                    PositionZ = c.PositionZ,
                    Layer = c.Layer,

                    Score = c.Score,
                    CanStack = !request.IsFragile
                })
                .ToList();

            _logger.LogInformation("Found {Count} suitable positions", result.Count);
            return result;
        }

        // Xác định container type dựa vào kích thước package
        private int DetermineContainerType(decimal length, decimal width, decimal height)
        {
            if (height > 0.5m && length <= 0.5m && width <= 0.5m)
                return 4;
            if (length <= 0.5m && width <= 0.5m && height <= 0.45m)
                return 1;
            if(length <= 0.75m && width <= 0.75m && height <= 0.45m)
                return 2;
            if (length <= 1.0m && width <= 0.5m && height <= 0.45m)
                return 3;
            return 0;
        }

        // Tìm vị trí cho Type D (floor 4)
        private async Task<List<PlacementCandidate>> FindPositionsForTypeD(
            List<Container> availableContainers,
            FindContainerRequest request,
            ContainerType containerType)
        {
            var candidates = new List<PlacementCandidate>();   
            //Get all floor 4
            var floors = await _unitOfWork.Floors.GetByFloorNumbersAsync(new List<int> { 4 });
            foreach (var floor in floors)
            {
                var occupiedContainers = await _unitOfWork.Containers.GetByFloorCodeAsync(floor.FloorCode);
                var positions = GenerateTypePositions(containerType, floor, occupiedContainers, request.IsFragile);
                foreach(var position in positions)
                {
                    if (availableContainers.Count == 0) break;
                    var container = availableContainers.First();
                    availableContainers.RemoveAt(0);
                    var score = CalculateScore(floor, position.Layer, request, containerType);
                    candidates.Add(new PlacementCandidate
                    {
                        Container = container,
                        Floor = floor,
                        PositionX = position.X,
                        PositionY = position.Y,
                        PositionZ = position.Z,
                        Layer = position.Layer,
                        Score = score
                    });
                }
            }
            return candidates;
        }
        //Tìm vị trí cho Type A, B, C (floors 1-3)
        private async Task<List<PlacementCandidate>> FindPositionsForTypeABC(
            List<Container> availableContainers,
            FindContainerRequest request,
            ContainerType containerType)
        {
            var candidates = new List<PlacementCandidate>();

            // Get floors 1-3
            var floors = await _unitOfWork.Floors.GetByFloorNumbersAsync(new List<int> { 1, 2, 3 });
            foreach(var floor in floors)
            {
                var occupiedContainers = await _unitOfWork.Containers.GetByFloorCodeAsync(floor.FloorCode);
                var layer0Used = occupiedContainers
                    .Where(c => c.PositionY.GetValueOrDefault() < 0.3m)
                    .Sum(c => c.ContainerType.Length.GetValueOrDefault());
                var layer0Available = FLOOR_LENGTH_LIMIT - layer0Used;
                if(containerType.Length <= layer0Available)
                {
                    if (availableContainers.Count == 0) break;
                    var container  =availableContainers.First();
                    var posX = layer0Used + (containerType.Length.GetValueOrDefault() / 2);
                    var score = CalculateScore(floor, 0, request, containerType);
                    candidates.Add(new PlacementCandidate
                    {
                        Container = container,
                        Floor = floor,
                        PositionX = posX,
                        PositionY = 0.0m,
                        PositionZ = 0.5m,
                        Layer = 0,
                        Score = score
                    });
                    availableContainers.RemoveAt(0);
                }
            }
            return candidates;
        }

        // Generate positions cho type D
        private List<PositionInfo> GenerateTypePositions(
            ContainerType containerType,
            Floor floor,
            List<Container> occupiedContainers,
            bool isFragile)
        {
            var positions = new List<PositionInfo>();   
            var gridX = new[] { 0.25m, 0.85m, 1.45m };
            var gridZ = new[] { 0.25m, 0.75m };
            foreach (var x in gridX)
            {
                foreach (var z in gridZ)
                {
                    var atThisPosition = occupiedContainers
                        .Where(c => Math.Abs(c.PositionX.GetValueOrDefault() - x) < 0.1m
                            && Math.Abs(c.PositionZ.GetValueOrDefault() - z) < 0.1m)
                        .OrderBy(c => c.PositionY)
                        .ToList();

                    if (!atThisPosition.Any())
                    {
                        positions.Add(new PositionInfo { X = x, Y = 0.0m, Z = z, Layer = 0 });
                    }
                    else if (atThisPosition.Count == 1 && !isFragile)
                    {
                        var bottom = atThisPosition[0];
                        if (bottom.ProductType?.IsFragile != true)
                        {
                            positions.Add(new PositionInfo { X = x, Y = 0.82m, Z = z, Layer = 1 });
                        }
                    }
                }
            }
            return positions;
        }
        //Tính điểm cho vị trí
        private double CalculateScore(Floor floor, int layer, FindContainerRequest request, ContainerType containerType)
        {
            double score = 0;
            if(request.PackageWeight > 20 || request.StorageDays > 30)
            {
                score += floor.FloorNumber == 1 ? 30 : floor.FloorNumber == 2 ? 20 : 10;
            }
            else
            {
                score += floor.FloorNumber == 3 ? 30 : floor.FloorNumber == 2 ? 20 : 10;
            }
            score += layer == 0 ? 20 : 12;
            var volumeUtilization = (request.PackageLength * request.PackageWidth * request.PackageHeight) /
                                    (containerType.Length.GetValueOrDefault() * containerType.Width.GetValueOrDefault() * containerType.Height.GetValueOrDefault());
            score += (double)volumeUtilization * 20;
            if(request.IsFragile && layer == 1)
            {
                score += 15;
            }
            if(request.PackageWeight > 20 && layer == 0)
            {
                score += 15;
            }
            return score;
        }

        private class PlacementCandidate
        {
            public Container Container { get; set; }
            public Floor Floor { get; set; }
            public decimal PositionX { get; set; }
            public decimal PositionY { get; set; }
            public decimal PositionZ { get; set; }
            public int Layer { get; set; }
            public double Score { get; set; }
        }

        private class PositionInfo
        {
            public decimal X { get; set; }
            public decimal Y { get; set; }
            public decimal Z { get; set; }
            public int Layer { get; set; }
        }
    }
}
