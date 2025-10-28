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
        public CLPService(IUnitOfWork unitOfWork, ILogger<CLPService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task<List<ContainerSuggestionDto>> FindSuitableContainersAsync(FindContainerRequest request)
        {
            var productType = await _unitOfWork.ProductType.GetEntityByIdAsync(request.ProductTypeID);
            if (productType == null)
            {
                _logger.LogWarning($"ProductType {request.ProductTypeID} not found");
                return new List<ContainerSuggestionDto>();
            }

            var candidates = await FindAllAvailableContainerCandidatesAsync(
                request.PackageLength,
                request.PackageWidth,
                request.PackageHeight,
                request.PackageWeight,
            productType,
                request.StorageDays,
                request.IsFragile
            );

            return candidates
                .OrderByDescending(c => c.Score)
                .Take(10)
                .Select(c => new ContainerSuggestionDto
                {
                    ContainerCode = c.Container.ContainerCode,
                    Score = c.Score,
                    Length = c.Container.Length ?? 0,
                    Width = c.Container.Width ?? 0,
                    Height = c.Container.Height ?? 0,
                    FloorCode = c.Floor.FloorCode,
                    FloorNumber = c.Floor.FloorNumber ?? 0,
                    ShelfCode = c.Shelf.ShelfCode,
                    StorageCode = c.Storage.StorageCode,
                    BuildingCode = c.Building.BuildingCode,
                    BuildingName = c.Building.Name ?? ""
                })
                .ToList();
        }

        private async Task<List<ContainerPlacementCandidate>> FindAllAvailableContainerCandidatesAsync(
            decimal packageLength,
            decimal packageWidth,
            decimal packageHeight,
            decimal packageWeight,
            ProductType productType,
            int storageDays,
            bool isFragile)
        {
            var candidates = new List<ContainerPlacementCandidate>();
            var suitableStorages = await _unitOfWork.Storages.GetWithFilterAsync(pageNumber: 1,
                pageSize: 10,
                buildingCode: null,
                storageTypeName: null,
                productTypeName: productType.Name);
            _logger.LogInformation($"Found {suitableStorages.Count} storages for ProductType {productType.Name}");
            if(!suitableStorages.Any())
            {
                return candidates;
            }
            foreach( var storage in suitableStorages.Where(s => s.IsActive == true && s.Status == "Active"))
            {
                var shelves = await _unitOfWork.Shelves.GetByStorageCodeAsync(storage.StorageCode);
                foreach( var shelf in shelves.Where(s => s.IsActive == true))
                {
                    var floors = await _unitOfWork.Floors.GetByShelfCodeAsync(shelf.ShelfCode);
                    foreach(var floor in floors.Where(f => f.IsActive == true))
                    {
                        var containersOnFloor = await _unitOfWork.Containers.GetByFloorCodeAsync(floor.FloorCode);
                        foreach(var container in containersOnFloor)
                        {
                            if(container.IsActive != true || container.Status != "Available")
                            {
                                continue;
                            }
                            if(container.Length < packageLength ||
                               container.Width < packageWidth ||
                               container.Height < packageHeight ||
                               (container.MaxWeight ?? 100) < packageWeight)
                            { continue; }

                            Building? building = null;
                            if (storage.BuildingId.HasValue)
                            {
                                building = await _unitOfWork.Building.GetEntityByIdAsync(storage.BuildingId.Value);
                            }
                            var score = CalculatePlacementScore(container, floor, packageWeight, storageDays, productType, isFragile);
                            candidates.Add(new ContainerPlacementCandidate
                            {
                                Container = container,
                                Floor = floor,
                                Shelf = shelf,
                                Storage = storage,
                                Building = building!,
                                Score = score
                            });
                        }
                    }
                }
            }
            _logger.LogInformation($"Found {candidates.Count} container candidates");
            return candidates;
        }

        private decimal CalculatePlacementScore(Container container, Floor floor, decimal packageWeight, int storageDays, ProductType productType, bool isFragile)
        {
            decimal score = 0;
            if(storageDays <= 30)
            {
                score += (floor.FloorNumber ?? 1) * 10;
            }
            else
            {
                score += (5 - (floor.FloorNumber ?? 1)) * 10;
            }

            var containerVolume = (container.Length ?? 0) * (container.Width ?? 0) * (container.Height ?? 0);
            if (containerVolume < 0.15m)
                score += 30;
            else if (containerVolume < 0.35m)
                score += 20;
            else if (containerVolume < 0.6m)
                score += 15;
            else
                score += 10;

            if (productType.IsFragile == true || isFragile)
            {
                score += (5 - (floor.FloorNumber ?? 1)) * 3;
            }
            if (packageWeight > 20)
            {
                score += (5 - (floor.FloorNumber ?? 1)) * 3;
            }
            return score;
        }
    }
}
