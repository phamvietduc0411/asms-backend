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
    }
}
