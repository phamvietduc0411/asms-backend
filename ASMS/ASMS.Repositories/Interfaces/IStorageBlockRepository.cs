using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASMS.Repositories.Entities;
using ASMS.Repositories.Infrastructures;

namespace ASMS.Repositories.Interfaces
{
    public interface IStorageBlockRepository : IGenericRepository<StorageBlock>
    {
        Task<List<StorageBlock>> GetWithFilterAsync(int pageNumber, int pageSize, string? storageCode);
        Task<int> GetTotalCountWithFilterAsync(string? storageCode);
        Task<StorageBlock?> GetByCodeAsync(string storageBlockCode);
        Task<bool> SoftDeleteAsync(string storageBlockCode);
    }
}
