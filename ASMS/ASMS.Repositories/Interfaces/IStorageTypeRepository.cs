using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASMS.Repositories.Entities;
using ASMS.Repositories.Infrastructures;

namespace ASMS.Repositories.Interfaces
{
    public interface IStorageTypeRepository : IGenericRepository<StorageType>
    {
        Task<List<StorageType>> GetWithFilterAsync(int pageNumber, int pageSize, string? nameContains);
        Task<int> GetTotalCountWithFilterAsync(string? nameContains);
        Task<StorageType?> GetByIdAsync(int id);
        Task<bool> DeleteAsync(int id);
    }
}
