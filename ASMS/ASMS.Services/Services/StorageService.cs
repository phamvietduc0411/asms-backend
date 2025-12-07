using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASMS.Repositories.Entities;
using ASMS.Repositories.Infrastructures;
using ASMS.Services.Interfaces;
using ASMS.Services.Model.Storages;
using AutoMapper;

namespace ASMS.Services.Services
{
    public class StorageService : IStorageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private static readonly int storageSmallCapacity = 10;
        private static readonly int storageMediumCapacity = 6;
        private static readonly int storageLargeCapacity = 4;
        private const string BUILDING_AC = "Self-Storage With AC";
        private const string BUILDING_NORMAL = "Self-Storage";
        private const string BUILDING_WAREHOUSE_PREFIX = "WareHouse";


        public StorageService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PaginatedStorageResponse> GetWithFilterAsync(int pageNumber, int pageSize, string? buildingCode, string? storageTypeName, string? productTypeName)
        {
            var storages = await _unitOfWork.Storages.GetWithFilterAsync(pageNumber, pageSize, buildingCode, storageTypeName, productTypeName);
            var totalCount = await _unitOfWork.Storages.GetTotalCountWithFilterAsync(buildingCode, storageTypeName, productTypeName);

            return new PaginatedStorageResponse
            {
                Data = _mapper.Map<List<StorageResponse>>(storages),
                Page = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };
        }

        public async Task<StorageResponse?> GetByCodeAsync(string storageCode)
        {
            var storage = await _unitOfWork.Storages.GetByCodeAsync(storageCode);
            return storage == null ? null : _mapper.Map<StorageResponse>(storage);
        }

        public async Task<StorageResponse> CreateAsync(CreateStorageRequest request)
        {
            var existing = await _unitOfWork.Storages.GetByCodeAsync(request.StorageCode);
            if (existing != null)
                throw new Exception($"Storage with code '{request.StorageCode}' already exists.");

            var isValid = await CanAddStorageToBuilding(request.BuildingId, request.StorageTypeId);
            if (!isValid)
                throw new Exception($"The number of storage has reached the maximum.");

            var storage = _mapper.Map<Storage>(request);
            storage.IsActive = true;

            var created = await _unitOfWork.Storages.AddAsync(storage);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<StorageResponse>(created);
        }

        public async Task<StorageResponse> UpdateAsync(string storageCode, UpdateStorageRequest request)
        {
            var existing = await _unitOfWork.Storages.GetByCodeAsync(storageCode);
            if (existing == null)
                throw new Exception($"Storage with code '{storageCode}' not found.");

            _mapper.Map(request, existing);
            await _unitOfWork.Storages.UpdateAsync(existing);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<StorageResponse>(existing);
        }

        public async Task<bool> ToggleActiveAsync(string storageCode)
        {
            var existing = await _unitOfWork.Storages.GetByCodeAsync(storageCode);
            if (existing == null)
                return false;

            if (existing.IsActive == true)
            {
                var hasRelatedData = await _unitOfWork.Storages.HasRelatedDataAsync(storageCode);
                if (hasRelatedData)
                    throw new Exception($"Cannot deactivate storage '{storageCode}' because it has related Shelves or Orders.");

                existing.IsActive = false;
            }
            else
            {
                existing.IsActive = true;
            }

            await _unitOfWork.Storages.UpdateAsync(existing);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        private async Task<bool> CanAddStorageToBuilding(int buildingId, int storageTypeId)
        {
            if (buildingId <= 0 || storageTypeId <= 0) return false;
            var type = await _unitOfWork.StorageTypes.GetEntityByIdAsync(storageTypeId);
            if (type == null) return false;
            var numberOfStorage = await _unitOfWork.Storages.GetNumberOfStorageWithBuildingCode(buildingId, type.Name);
            int capacity = 0;

            switch (type.Name)
            {
                case "Small":
                    capacity = storageSmallCapacity;
                    break;

                case "Medium":
                    capacity = storageMediumCapacity;
                    break;

                case "Large":
                    capacity = storageLargeCapacity;
                    break;

                default:
                    return false;
            }

            if (numberOfStorage < capacity)
                return true;

            return false;

        }
        /// <summary>
        /// Tính toán volume dựa trên container trong kho WareHouse
        /// </summary>
        private async Task<(decimal UsedVolume, int TotalContainers)> CalculateContainerBasedVolumeAsync(string storageCode)
        {
            try
            {
                var shelves = await _unitOfWork.Shelves.GetByStorageCodeAsNoTrackingAsync(storageCode);

                decimal totalUsedVolume = 0;
                int totalContainers = 0;

                foreach (var shelf in shelves)
                {
                    var floors = await _unitOfWork.Floors.GetByShelfCodeAsNoTrackingAsync(shelf.ShelfCode);

                    foreach (var floor in floors)
                    {
                        var containers = await _unitOfWork.Containers.GetByFloorCodeAsNoTrackingAsync(floor.FloorCode);

                        foreach (var container in containers)
                        {
                            if (container.ContainerType != null)
                            {
                                var containerVolume =
                                    (container.ContainerType.Length ?? 0) *
                                    (container.ContainerType.Width ?? 0) *
                                    (container.ContainerType.Height ?? 0);

                                totalUsedVolume += containerVolume;
                                totalContainers++;
                            }
                        }
                    }
                }

                return (totalUsedVolume, totalContainers);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        /// <summary>
        /// Tính toán volume cho một storage (không cập nhật database)
        /// </summary>
        public async Task<StorageVolumeCalculationResult> CalculateStorageVolumeAsync(string storageCode)
        {
            try
            {
                var storage = await _unitOfWork.Storages.GetByCodeAsNoTrackingAsync(storageCode);
                if (storage == null)
                {
                    return null;
                }

                var building = storage.Building;
                if (building == null)
                {
                    return null;
                }

                var totalVolume = storage.TotalVolume ??
                    (storage.Length * storage.Width * storage.Height) ?? 0;

                var result = new StorageVolumeCalculationResult
                {
                    StorageCode = storageCode,
                    StorageName = storage.StorageType?.Name ?? storageCode,
                    BuildingName = building.Name,
                    Status = storage.Status,
                    TotalVolume = totalVolume
                };

                // === CASE 1: Self-Storage hoặc Self-Storage With AC ===
                if (building.Name == BUILDING_AC || building.Name == BUILDING_NORMAL)
                {
                    if (storage.Status == "Reserved")
                    {
                        result.UsedVolume = totalVolume;
                        result.UtilizationRate = 100m;
                        result.IsReserved = true;
                        result.CalculationMethod = "Reserved (100% occupied)";

                    }
                    else
                    {
                        result.UsedVolume = 0;
                        result.UtilizationRate = 0;
                        result.IsReserved = false;
                        result.CalculationMethod = "Not Reserved (0% occupied)";

                    }
                }
                // === CASE 2: WareHouse (bắt đầu bằng "WareHouse") ===
                else if (building.Name?.StartsWith(BUILDING_WAREHOUSE_PREFIX) == true)
                {
                    var volumeData = await CalculateContainerBasedVolumeAsync(storageCode);

                    result.UsedVolume = volumeData.UsedVolume;
                    result.UtilizationRate = totalVolume > 0
                        ? Math.Round((volumeData.UsedVolume / totalVolume) * 100, 2)
                        : 0;
                    result.TotalContainers = volumeData.TotalContainers;
                    result.IsReserved = false;
                    result.CalculationMethod = $"Container-based ({volumeData.TotalContainers} containers)";

                }
                else
                {
                    result.UsedVolume = 0;
                    result.UtilizationRate = 0;
                    result.CalculationMethod = "Unknown building type";
                }

                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Tính toán và cập nhật volume cho một storage
        /// </summary>
        public async Task<StorageVolumeCalculationResult> CalculateAndUpdateStorageVolumeAsync(string storageCode)
        {
            try
            {
                var result = await CalculateStorageVolumeAsync(storageCode);

                if (result == null)
                {
                    return null;
                }

                // Cập nhật vào database
                var storage = await _unitOfWork.Storages.GetByCodeWithoutIncludesAsync(storageCode);
                if (storage != null)
                {
                    storage.UsedVolume = result.UsedVolume;
                    storage.UtilizationRate = result.UtilizationRate;
                    storage.TotalContainers = result.TotalContainers;
                    storage.LastOptimizedDate = DateTime.Now;

                    await _unitOfWork.Storages.UpdateAsync(storage);
                    await _unitOfWork.CompleteAsync();


                }

                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        /// <summary>
        /// Tính toán và cập nhật volume cho TẤT CẢ storages
        /// </summary>
        public async Task<BatchVolumeCalculationResult> CalculateAndUpdateAllStorageVolumesAsync()
        {
            var result = new BatchVolumeCalculationResult();

            try
            {
                var storages = await _unitOfWork.Storages.GetAllAsNoTrackingAsync();
                result.TotalStorages = storages.Count;


                var storageUpdates = new List<(string StorageCode, decimal UsedVolume, decimal UtilizationRate, int TotalContainers)>();

                foreach (var storage in storages)
                {
                    try
                    {
                        var calculationResult = await CalculateStorageVolumeAsync(storage.StorageCode);

                        if (calculationResult != null)
                        {
                            result.Details.Add(calculationResult);

                            storageUpdates.Add((
                                storage.StorageCode,
                                calculationResult.UsedVolume,
                                calculationResult.UtilizationRate,
                                calculationResult.TotalContainers
                            ));
                        }
                        else
                        {
                            result.FailedStorages++;
                            result.Errors.Add($"Failed to calculate volume for storage {storage.StorageCode}");
                        }
                    }
                    catch (Exception ex)
                    {
                        result.FailedStorages++;
                        result.Errors.Add($"Error for storage {storage.StorageCode}: {ex.Message}");
                    }
                }

                if (storageUpdates.Any())
                {
                    await BatchUpdateStoragesAsync(storageUpdates);
                    result.UpdatedStorages = storageUpdates.Count;
                }

                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        /// <summary>
        /// ✅ BATCH UPDATE STORAGES - CẬP NHẬT HÀNG LOẠT
        /// </summary>
        private async Task BatchUpdateStoragesAsync(
            List<(string StorageCode, decimal UsedVolume, decimal UtilizationRate, int TotalContainers)> updates)
        {
            foreach (var update in updates)
            {
                var storage = await _unitOfWork.Storages.GetByCodeWithoutIncludesAsync(update.StorageCode);

                if (storage != null)
                {
                    storage.UsedVolume = update.UsedVolume;
                    storage.UtilizationRate = update.UtilizationRate;
                    storage.TotalContainers = update.TotalContainers;
                    storage.LastOptimizedDate = DateTime.Now;

                    await _unitOfWork.Storages.UpdateAsync(storage);
                }
            }
            await _unitOfWork.CompleteAsync();
        }
    }
}
