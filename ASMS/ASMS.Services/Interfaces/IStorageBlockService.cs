using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASMS.Services.Model.StorageBlocks;

namespace ASMS.Services.Interfaces
{
    public interface IStorageBlockService
    {
        Task<PaginatedStorageBlockResponse> GetWithFilterAsync(int pageNumber, int pageSize, string? storageCode);
        Task<StorageBlockResponse?> GetByCodeAsync(string storageBlockCode);
        Task<StorageBlockResponse> CreateAsync(CreateStorageBlockRequest request);
        Task<StorageBlockResponse> UpdateAsync(string storageBlockCode, UpdateStorageBlockRequest request);
        Task<bool> ToggleActiveAsync(string storageBlockCode, bool isActive); 
        Task<bool> SoftDeleteAsync(string storageBlockCode);
    }
}
