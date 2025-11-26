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
using static System.Formats.Asn1.AsnWriter;

namespace ASMS.Services.Services
{
    public class CLPService : ICLPService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CLPService> _logger;

        // Kích thước kệ đơn thực tế: 1.7m (D) × 1.07m (R) × 5.2m (C)
        private const decimal SHELF_LENGTH = 1.7m;   // Chiều dài kệ (trục X)
        private const decimal SHELF_DEPTH = 1.07m;   // Chiều rộng kệ (trục Z)

        // Chiều cao mỗi tầng
        private const decimal FLOOR_HEIGHT_123 = 1.2m;  // Tầng 1, 2, 3
        private const decimal FLOOR_HEIGHT_4 = 1.6m;    // Tầng 4
        private const int MAX_CONTAINERS_PER_FLOOR_ABC = 12; 
        private const int MAX_CONTAINERS_PER_FLOOR_D = 2;

        // Tên các building
        private const string BUILDING_NORMAL = "WareHouse";
        private const string BUILDING_AC = "WareHouse With AC";
        private const string BUILDING_OVERSIZED = "WareHouse Oversized";

        public CLPService(IUnitOfWork unitOfWork, ILogger<CLPService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        /// <summary>
        /// Tìm container và vị trí phù hợp cho package
        /// Input: Danh sách ProductType thay vì 1 ProductType
        /// </summary>
        public async Task<List<ContainerPlacementDto>> FindSuitableContainersAsync(FindContainerRequest request)
        {
            _logger.LogInformation("Finding suitable containers for package: {L}x{W}x{H}, Weight: {Wt}kg, ProductTypes: [{Types}]",
                request.PackageLength, request.PackageWidth, request.PackageHeight, request.PackageWeight,
                string.Join(", ", request.ProductTypeIds ?? new List<int>()));

            // 1. Xác định building phù hợp dựa vào ProductType
            var targetBuilding = await DetermineBuildingAsync(request);
            if (targetBuilding == null)
            {
                _logger.LogWarning("Cannot determine suitable building for package");
                return new List<ContainerPlacementDto>();
            }

            _logger.LogInformation("Target building: {Building}", targetBuilding.Name);

            // 2. Kiểm tra hàng quá cỡ
            if (targetBuilding.Name == BUILDING_OVERSIZED)
            {
                _logger.LogInformation("Package is oversized, placed in {Building}", BUILDING_OVERSIZED);
                return new List<ContainerPlacementDto>
                {
                    new ContainerPlacementDto
                    {
                        BuildingName = BUILDING_OVERSIZED,
                        BuildingId = targetBuilding.BuildingId,
                        IsOversized = true,
                        Score = 100
                    }
                };
            }

            // 3. Xác định loại container cần dùng
            var containerTypeId = DetermineContainerType(request.PackageLength, request.PackageWidth, request.PackageHeight);
            if (containerTypeId == 0)
            {
                _logger.LogWarning("Package dimensions {L}x{W}x{H} too large for any container type",
                    request.PackageLength, request.PackageWidth, request.PackageHeight);
                return new List<ContainerPlacementDto>();
            }

            var containerType = await _unitOfWork.ContainerType.GetByIdAsync(containerTypeId);
            if (containerType == null)
            {
                _logger.LogWarning("Container type {TypeId} not found in database", containerTypeId);
                return new List<ContainerPlacementDto>();
            }

            // 4. Kiểm tra hàng có dễ vỡ không
            var isFragile = await IsPackageFragileAsync(request.ProductTypeIds);

            _logger.LogInformation("Package is fragile: {Fragile}", isFragile);

            // 5. Lấy danh sách container trống của loại này trong building phù hợp
            var availableContainers = await _unitOfWork.Containers.GetAvailableByTypeAsync(containerTypeId);

            if (!availableContainers.Any())
            {
                _logger.LogWarning("No available containers of type {Type} in building {Building}",
                    containerType.Type, targetBuilding.Name);
                return new List<ContainerPlacementDto>();
            }

            // 6. Tìm vị trí phù hợp
            var candidates = new List<PlacementCandidate>();

            if (containerTypeId == 4) // Type D - chỉ tầng 4
            {
                candidates = await FindPositionsForTypeD(
                    availableContainers,
                    request,
                    containerType,
                    isFragile,
                    targetBuilding);
            }
            else // Type A, B, C - tầng 1-3
            {
                candidates = await FindPositionsForTypeABC(
                    availableContainers,
                    request,
                    containerType,
                    isFragile,
                    targetBuilding);
            }

            // 7. Sắp xếp theo điểm và trả về top 10
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
                    BuildingName = targetBuilding.Name,
                    BuildingId = targetBuilding.BuildingId,

                    // Tọa độ X, Y, Z để Frontend tính toán
                    PositionX = 0,
                    PositionY = 0,
                    PositionZ = 0,

                    Layer = c.Layer,
                    SerialNumber = c.SerialNumber,
                    ContainerAboveCode = c.ContainerAboveCode,

                    Score = c.Score,
                    IsFragile = isFragile,

                    // Thông tin tái sắp xếp
                    RequiresRearrangement = c.RequiresRearrangement,
                    RearrangeContainerCode = c.RearrangeContainerCode
                })
                .ToList();

            _logger.LogInformation("Found {Count} suitable positions (including {Rearrange} rearrangement options)",
                result.Count, result.Count(r => r.RequiresRearrangement));

            return result;
        }

        /// <summary>
        /// Xác định building phù hợp dựa vào ProductType
        /// - Hàng lạnh → WareHouse With AC
        /// - Hàng quá cỡ → WareHouse Oversized
        /// - Hàng bình thường → WareHouse
        /// </summary>
        private async Task<Building> DetermineBuildingAsync(FindContainerRequest request)
        {
            // Kiểm tra hàng quá cỡ (không vừa bất kỳ container nào)
            var containerTypeId = DetermineContainerType(
                request.PackageLength,
                request.PackageWidth,
                request.PackageHeight);

            if (containerTypeId == 0) // Quá cỡ
            {
                return await _unitOfWork.Building
                    .GetByNameAsync(BUILDING_OVERSIZED);
            }

            // Kiểm tra ProductType để xác định có cần kho lạnh không
            if (request.ProductTypeIds != null && request.ProductTypeIds.Any())
            {
                var productTypes = await _unitOfWork.ProductType
                    .GetByIdsAsync(request.ProductTypeIds);

                // Kiểm tra có ProductType nào cần kho lạnh không
                var requiresAC = productTypes.Any(pt =>
                    pt.Name != null && pt.Name.Contains("lạnh", StringComparison.OrdinalIgnoreCase));

                if (requiresAC)
                {
                    return await _unitOfWork.Building
                        .GetByNameAsync(BUILDING_AC);
                }
            }

            // Mặc định: Kho bình thường
            return await _unitOfWork.Building
                .GetByNameAsync(BUILDING_NORMAL);
        }

        /// <summary>
        /// Kiểm tra package có dễ vỡ không dựa vào danh sách ProductType
        /// </summary>
        private async Task<bool> IsPackageFragileAsync(List<int> productTypeIds)
        {
            if (productTypeIds == null || !productTypeIds.Any())
                return false;

            var productTypes = await _unitOfWork.ProductType
                .GetByIdsAsync(productTypeIds);

            // Nếu có bất kỳ ProductType nào là dễ vỡ → Package dễ vỡ
            return productTypes.Any(pt => pt.IsFragile == true);
        }


        /// <summary>
        /// Xác định loại container phù hợp dựa vào kích thước package
        /// </summary>
        private int DetermineContainerType(decimal length, decimal width, decimal height)
        {
            // Type D: Vertical - cao hơn
            if (height > 0.5m && length <= 0.5m && width <= 0.5m)
                return 4;

            // Type A: Small
            if (length <= 0.5m && width <= 0.5m && height <= 0.45m)
                return 1;

            // Type B: Medium
            if (length <= 0.75m && width <= 0.75m && height <= 0.45m)
                return 2;

            // Type C: Large
            if (length <= 1.0m && width <= 0.5m && height <= 0.45m)
                return 3;

            return 0; // Quá cỡ
        }

        /// <summary>
        /// Tìm vị trí cho Type D (tầng 4)
        /// </summary>
        private async Task<List<PlacementCandidate>> FindPositionsForTypeD(
    List<Container> availableContainers,
    FindContainerRequest request,
    ContainerType containerType,
    bool isFragile,
    Building building)
        {
            var candidates = new List<PlacementCandidate>();

            // Lấy tất cả tầng 4 trong building này
            var allFloors = await GetFloorsInBuildingAsync(
                building.BuildingId,
                new List<int> { 4 });

            var floorsByShelf = allFloors
                .GroupBy(f => f.ShelfCode)
                .OrderBy(g => g.Key)
                .ToList();

            int shelfIndex = 0;

            foreach (var shelfGroup in floorsByShelf)
            {
                if (availableContainers.Count == 0) break;

                var floorsInShelf = shelfGroup.OrderBy(f => f.FloorNumber).ToList();
                int floorIndex = 0;

                foreach (var floor in floorsInShelf)
                {
                    if (availableContainers.Count == 0) break;

                    var occupiedContainers = await _unitOfWork.Containers
                        .GetByFloorCodeAsync(floor.FloorCode);

                    if (occupiedContainers.Count >= MAX_CONTAINERS_PER_FLOOR_D)
                    {
                        _logger.LogDebug("Floor {Floor} (Type D) is full ({Count}/{Max})",
                            floor.FloorCode, occupiedContainers.Count, MAX_CONTAINERS_PER_FLOOR_D);
                        floorIndex++;
                        continue;
                    }


                    int nextSerial = occupiedContainers.Any()
                        ? occupiedContainers.Max(c => c.SerialNumber ?? 0) + 1
                        : 1;

                    var availableSpots = CalculateAvailableSpotsTypeD(
                        occupiedContainers,
                        containerType,
                        isFragile);

                    _logger.LogInformation("Floor {Floor} (Type D): {Occupied}/{Max} containers, {Spots} spots, Next serial: {Serial}",
                        floor.FloorCode, occupiedContainers.Count, MAX_CONTAINERS_PER_FLOOR_D,
                        availableSpots.Count, nextSerial);

                    int remainingCapacity = MAX_CONTAINERS_PER_FLOOR_D - occupiedContainers.Count;
                    var limitedSpots = availableSpots.Take(remainingCapacity).ToList();

                    foreach (var spot in limitedSpots)
                    {
                        if (availableContainers.Count == 0) break;

                        var container = availableContainers.First();
                        availableContainers.RemoveAt(0);

                        var score = CalculateScore(floor, spot.Layer, request, containerType, isFragile, shelfIndex, floorIndex);

                        if (spot.IsRearrangement)
                        {
                            score += 5;
                            _logger.LogInformation("Type D Rearrangement: Container {New} will replace fragile {Old}",
                                container.ContainerCode, spot.RearrangeContainerCode);
                        }

                        candidates.Add(new PlacementCandidate
                        {
                            Container = container,
                            Floor = floor,
                            Layer = spot.Layer,
                            SerialNumber = nextSerial, 
                            ContainerAboveCode = spot.ContainerAboveCode,
                            Score = score,
                            RequiresRearrangement = spot.IsRearrangement,
                            RearrangeContainerCode = spot.RearrangeContainerCode
                        });

                    }
                    floorIndex++;
                }
                shelfIndex++;
            }
            return candidates;
        }


        /// <summary>
        /// Tìm vị trí cho Type A, B, C (tầng 1-3)
        /// Các loại này CÓ THỂ XEN KẼ
        /// </summary>
        private async Task<List<PlacementCandidate>> FindPositionsForTypeABC(
    List<Container> availableContainers,
    FindContainerRequest request,
    ContainerType containerType,
    bool isFragile,
    Building building)
        {
            var candidates = new List<PlacementCandidate>();

            // Lấy tầng 1-3 trong building này
            var allFloors = await GetFloorsInBuildingAsync(
                building.BuildingId,
                new List<int> { 1, 2, 3 });

 
            var floorsByShelf = allFloors
                .GroupBy(f => f.ShelfCode)
                .OrderBy(g => g.Key) // Xếp theo thứ tự shelf
                .ToList();

            int shelfIndex = 0;
            // Duyệt qua từng SHELF
            foreach (var shelfGroup in floorsByShelf)
            {
                if (availableContainers.Count == 0) break;

                // Lấy các tầng của shelf này (đã sắp xếp 1, 2, 3)
                var floorsInShelf = shelfGroup.OrderBy(f => f.FloorNumber).ToList();
                int floorIndex = 0;
                foreach (var floor in floorsInShelf)
                {
                    if (availableContainers.Count == 0) break;

                    // Lấy TẤT CẢ container đã xếp trên tầng này
                    var occupiedContainers = await _unitOfWork.Containers
                        .GetByFloorCodeAsync(floor.FloorCode);
                    
                    // ✅ KIỂM TRA TẦNG ĐÃ ĐẦY CHƯA
                    if (occupiedContainers.Count >= MAX_CONTAINERS_PER_FLOOR_ABC)
                    {
                        _logger.LogDebug("Floor {Floor} is full ({Count}/{Max}), skip to next floor",
                            floor.FloorCode, occupiedContainers.Count, MAX_CONTAINERS_PER_FLOOR_ABC);
                        floorIndex++;
                        continue; // Bỏ qua tầng đầy
                    }

                    int nextSerial = occupiedContainers.Any()
                        ? occupiedContainers.Max(c => c.SerialNumber ?? 0) + 1
                        : 1;

                    // Tính diện tích đã sử dụng
                    var usedArea = CalculateUsedAreaOnFloor(occupiedContainers);

                    // Tính số vị trí trống
                    var availableSpots = CalculateAvailableSpotsTypeABC(
                        occupiedContainers,
                        containerType,
                        isFragile,
                        usedArea);

                    _logger.LogInformation("Floor {Floor}: {Occupied}/{Max} containers, {Spots} available spots, Next serial: {Serial}",
                        floor.FloorCode, occupiedContainers.Count, MAX_CONTAINERS_PER_FLOOR_ABC,
                        availableSpots.Count, nextSerial);

                    int remainingCapacity = MAX_CONTAINERS_PER_FLOOR_ABC - occupiedContainers.Count;
                    var limitedSpots = availableSpots.Take(remainingCapacity).ToList();

                    // Phân bổ container vào các vị trí
                    foreach (var spot in limitedSpots)
                    {
                        if (availableContainers.Count == 0) break;

                        var container = availableContainers.First();
                        availableContainers.RemoveAt(0);

                        var score = CalculateScore(floor, spot.Layer, request, containerType, isFragile, shelfIndex, floorIndex);

                        if (spot.IsRearrangement)
                        {
                            score += 5;
                            _logger.LogInformation("Type ABC Rearrangement: Container {New} will replace fragile {Old}",
                                container.ContainerCode, spot.RearrangeContainerCode);
                        }

                        candidates.Add(new PlacementCandidate
                        {
                            Container = container,
                            Floor = floor,
                            Layer = spot.Layer,
                            SerialNumber = nextSerial, 
                            ContainerAboveCode = spot.ContainerAboveCode,
                            Score = score,
                            RequiresRearrangement = spot.IsRearrangement,
                            RearrangeContainerCode = spot.RearrangeContainerCode
                        });

                    }

                    if (occupiedContainers.Count + limitedSpots.Count < MAX_CONTAINERS_PER_FLOOR_ABC)
                    {
                        _logger.LogDebug("Floor {Floor} not full yet, continue filling this floor",
                            floor.FloorCode);

                    }
                    floorIndex++;
                }
                shelfIndex++;
            }

            return candidates;
        }
        private async Task<List<Floor>> GetFloorsInBuildingAsync(int buildingId, List<int> floorNumbers)
        {
            var allFloors = await _unitOfWork.Floors.GetByFloorNumbersAsync(floorNumbers);
            return allFloors.Where(f => f.ShelfCodeNavigation?.StorageCodeNavigation?.BuildingId == buildingId)
                .OrderBy(f => f.FloorNumber)
                .ToList();
        }
        /// <summary>
        /// Tính diện tích đã sử dụng trên 1 tầng
        /// </summary>
        private FloorUsageInfo CalculateUsedAreaOnFloor(List<Container> occupiedContainers)
        {
            var info = new FloorUsageInfo();

            foreach (var container in occupiedContainers)
            {
                var containerLength = container.ContainerType?.Length ?? 0;
                var containerWidth = container.ContainerType?.Width ?? 0;
                var containerArea = containerLength * containerWidth;
                var layer = container.Layer ?? 0;

                if (layer == 0) // Layer 0
                {
                    info.Layer0UsedArea += containerArea;
                    info.Layer0Count++;

                    // Đếm container dễ vỡ ở Layer 0
                    // KIỂM TRA ProductType CỦA CONTAINER
                    if (container.ProductType?.IsFragile == true)
                    {
                        info.Layer0FragileArea += containerArea;
                        info.Layer0FragileContainers.Add(container);
                    }
                }
                else // Layer 1
                {
                    info.Layer1UsedArea += containerArea;
                    info.Layer1Count++;
                }
            }

            return info;
        }

        /// <summary>
        /// Tính số vị trí trống cho Type A, B, C
        /// HỖ TRỢ TÁI SẮP XẾP ĐỘNG
        /// </summary>
        private List<SpotInfo> CalculateAvailableSpotsTypeABC(
    List<Container> occupiedContainers,
    ContainerType containerType,
    bool isFragile,
    FloorUsageInfo usedArea)
        {
            var spots = new List<SpotInfo>();

            var totalShelfArea = SHELF_LENGTH * SHELF_DEPTH; // 1.819 m²
            var containerArea = containerType.Length.GetValueOrDefault() *
                              containerType.Width.GetValueOrDefault();

            // === LAYER 0 ===
            var availableAreaLayer0 = totalShelfArea - usedArea.Layer0UsedArea;
            var maxContainersLayer0 = (int)(availableAreaLayer0 / containerArea);


            var currentLayer0Count = occupiedContainers.Count(c => c.Layer == 0);
            var maxLayer0Capacity = 6;

            maxContainersLayer0 = Math.Min(maxContainersLayer0, maxLayer0Capacity - currentLayer0Count);
            maxContainersLayer0 = Math.Max(0, maxContainersLayer0);

            _logger.LogDebug("Layer 0: Available area = {Area:F3} m², Can fit {Count} containers (limit: {Limit})",
                availableAreaLayer0, maxContainersLayer0, maxLayer0Capacity);

            for (int i = 0; i < maxContainersLayer0; i++)
            {
                spots.Add(new SpotInfo { Layer = 0 });
            }

            // === TÁI SẮP XẾP ĐỘNG ===
            if (!isFragile && maxContainersLayer0 == 0 && usedArea.Layer0FragileContainers.Any())
            {
                var nonFragileLayer0Containers = occupiedContainers
                    .Where(c => c.Layer == 0 && c.ProductType?.IsFragile != true)
                    .ToList();

                var stackableArea = nonFragileLayer0Containers.Sum(c =>
                    (c.ContainerType?.Length ?? 0) * (c.ContainerType?.Width ?? 0));

                var availableLayer1Area = stackableArea - usedArea.Layer1UsedArea;

                foreach (var fragileContainer in usedArea.Layer0FragileContainers)
                {
                    var fragileArea = (fragileContainer.ContainerType?.Length ?? 0) *
                                    (fragileContainer.ContainerType?.Width ?? 0);

                    if (containerArea >= fragileArea && availableLayer1Area >= fragileArea)
                    {
                        spots.Add(new SpotInfo
                        {
                            Layer = 0,
                            IsRearrangement = true,
                            RearrangeContainerCode = fragileContainer.ContainerCode,
                            ContainerAboveCode = null
                        });

                        availableLayer1Area -= fragileArea;

                        _logger.LogInformation("Rearrangement available: Push {Fragile} to Layer 1",
                            fragileContainer.ContainerCode);
                    }
                }
            }

            // === LAYER 1 ===
            if (!isFragile)
            {
                var nonFragileLayer0Containers = occupiedContainers
                    .Where(c => c.Layer == 0
                           && c.ProductType?.IsFragile != true
                           && string.IsNullOrEmpty(c.ContainerAboveCode))
                    .ToList();

                var stackableArea = nonFragileLayer0Containers.Sum(c =>
                    (c.ContainerType?.Length ?? 0) * (c.ContainerType?.Width ?? 0));

                var availableStackableArea = stackableArea - usedArea.Layer1UsedArea;
                var maxContainersLayer1 = (int)(availableStackableArea / containerArea);


                var currentLayer1Count = occupiedContainers.Count(c => c.Layer == 1);
                var maxLayer1Capacity = 7;
                maxContainersLayer1 = Math.Min(maxContainersLayer1, maxLayer1Capacity - currentLayer1Count);
                maxContainersLayer1 = Math.Max(0, maxContainersLayer1);

                _logger.LogDebug("Layer 1: Stackable area = {Area:F3} m², Can fit {Count} containers (limit: {Limit})",
                    availableStackableArea, maxContainersLayer1, maxLayer1Capacity);

                for (int i = 0; i < maxContainersLayer1; i++)
                {
                    spots.Add(new SpotInfo
                    {
                        Layer = 1,
                        ContainerAboveCode = null
                    });
                }
            }

            return spots;
        }

        /// <summary>
        /// Tính số vị trí trống cho Type D (tầng 4)
        /// Logic tương tự Type ABC
        /// </summary>
        private List<SpotInfo> CalculateAvailableSpotsTypeD(
            List<Container> occupiedContainers,
            ContainerType containerType,
            bool isFragile)
        {
            var spots = new List<SpotInfo>();

            // Tính theo grid
            var containersPerRow = (int)(SHELF_LENGTH / containerType.Length.GetValueOrDefault());
            var rowsPerShelf = (int)(SHELF_DEPTH / containerType.Width.GetValueOrDefault());
            var totalSpotsLayer0 = containersPerRow * rowsPerShelf;

            // Đếm theo Layer (SỬ DỤNG THUỘC TÍNH Layer)
            var occupiedLayer0 = occupiedContainers.Count(c => c.Layer == 0);
            var occupiedLayer1 = occupiedContainers.Count(c => c.Layer == 1);

            _logger.LogDebug("Type D - Layer 0: {Occupied}/{Total} positions occupied",
                occupiedLayer0, totalSpotsLayer0);

            // Layer 0 - Vị trí trống
            var availableLayer0 = totalSpotsLayer0 - occupiedLayer0;
            for (int i = 0; i < availableLayer0; i++)
            {
                spots.Add(new SpotInfo { Layer = 0 });
            }

            // === TÁI SẮP XẾP ĐỘNG ===
            if (!isFragile && availableLayer0 == 0)
            {
                var fragileLayer0Containers = occupiedContainers
                    .Where(c => c.Layer == 0 && c.ProductType?.IsFragile == true)
                    .ToList();

                if (fragileLayer0Containers.Any())
                {
                    var nonFragileLayer0Count = occupiedContainers
                        .Count(c => c.Layer == 0
                               && c.ProductType?.IsFragile != true
                               && string.IsNullOrEmpty(c.ContainerAboveCode));

                    var availableLayer1Spots = Math.Max(0, nonFragileLayer0Count - occupiedLayer1);

                    foreach (var fragileContainer in fragileLayer0Containers.Take(availableLayer1Spots))
                    {
                        spots.Add(new SpotInfo
                        {
                            Layer = 0,
                            IsRearrangement = true,
                            RearrangeContainerCode = fragileContainer.ContainerCode
                        });

                        _logger.LogInformation("Type D Rearrangement: Can push {Code} to Layer 1",
                            fragileContainer.ContainerCode);
                    }
                }
            }

            // Layer 1 - Xếp chồng
            if (!isFragile)
            {
                var nonFragileLayer0Count = occupiedContainers
                    .Count(c => c.Layer == 0
                           && c.ProductType?.IsFragile != true
                           && string.IsNullOrEmpty(c.ContainerAboveCode));

                var availableLayer1 = Math.Max(0, nonFragileLayer0Count - occupiedLayer1);

                _logger.LogDebug("Type D - Layer 1: {Available} positions available",
                    availableLayer1);

                for (int i = 0; i < availableLayer1; i++)
                {
                    spots.Add(new SpotInfo { Layer = 1 });
                }
            }

            return spots;
        }

        /// <summary>
        /// Tính điểm ưu tiên cho vị trí
        /// </summary>
        private double CalculateScore(
            Floor floor,
            int layer,
            FindContainerRequest request,
            ContainerType containerType,
            bool isFragile,
            int shelfPriority = 0,
            int floorPriority = 0)
        {
            double score = 0;

            // === 0. ƯU TIÊN SHELF ===
            score += (1000 - shelfPriority * 300);  

            // === 0.5. ƯU TIÊN FLOOR TRONG SHELF ===
            score += (200 - floorPriority * 50);    

            // === 1. Ưu tiên tầng theo trọng lượng ===
            if (request.PackageWeight > 20 || request.StorageDays > 30)
            {
                // Hàng nặng → ưu tiên tầng thấp
                score += floor.FloorNumber == 1 ? 30 :
                         floor.FloorNumber == 2 ? 25 :
                         floor.FloorNumber == 3 ? 20 : 15;
            }
            else
            {
                // Hàng nhẹ → ưu tiên tầng cao
                score += floor.FloorNumber == 3 ? 30 :
                         floor.FloorNumber == 2 ? 25 :
                         floor.FloorNumber == 1 ? 20 : 15;
            }

            // === 2. Ưu tiên Layer 0 ===
            score += layer == 0 ? 20 : 12;

            // === 3. Điểm sử dụng thể tích ===
            var packageVolume = request.PackageLength * request.PackageWidth * request.PackageHeight;
            var containerVolume = containerType.Length.GetValueOrDefault() *
                                 containerType.Width.GetValueOrDefault() *
                                 containerType.Height.GetValueOrDefault();
            var volumeUtilization = (double)(packageVolume / containerVolume);
            score += volumeUtilization * 20;

            // === 4. Phạt nếu hàng dễ vỡ ở Layer 1 ===
            if (isFragile && layer == 1)
            {
                score -= 5;
            }

            // === 5. Thưởng nếu hàng nặng ở Layer 0 ===
            if (request.PackageWeight > 20 && layer == 0)
            {
                score += 15;
            }

            return score;
        }

        // === PRIVATE CLASSES ===

        private class PlacementCandidate
        {
            public Container Container { get; set; }
            public Floor Floor { get; set; }
            public int Layer { get; set; }
            public int SerialNumber { get; set; }
            public string ContainerAboveCode { get; set; }
            public double Score { get; set; }
            public bool RequiresRearrangement { get; set; }
            public string RearrangeContainerCode { get; set; }
        }

        private class SpotInfo
        {
            public int Layer { get; set; }
            public bool IsRearrangement { get; set; }
            public string RearrangeContainerCode { get; set; }
            public string ContainerAboveCode { get; set; }
        }

        private class FloorUsageInfo
        {
            public decimal Layer0UsedArea { get; set; }
            public decimal Layer1UsedArea { get; set; }
            public int Layer0Count { get; set; }
            public int Layer1Count { get; set; }
            public decimal Layer0FragileArea { get; set; }
            public List<Container> Layer0FragileContainers { get; set; } = new List<Container>();
        }
    }
}
