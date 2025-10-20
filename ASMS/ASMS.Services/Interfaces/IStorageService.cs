using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASMS.Services.Model.Storages;

namespace ASMS.Services.Interfaces
{
    public interface IStorageService
    {
        Task<PaginatedStorageResponse> GetWithFilterAsync(int pageNumber, int pageSize, string? buildingCode, string? storageTypeName, string? productTypeName);
        Task<StorageResponse?> GetByCodeAsync(string storageCode);
        Task<StorageResponse> CreateAsync(CreateStorageRequest request);
        Task<StorageResponse> UpdateAsync(string storageCode, UpdateStorageRequest request);
        Task<bool> ToggleActiveAsync(string storageCode);
    }
}
