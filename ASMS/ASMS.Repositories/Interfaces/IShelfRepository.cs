using ASMS.Repositories.Common;
using ASMS.Repositories.Entities;
using ASMS.Repositories.Infrastructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Repositories.Interfaces
{
    public interface IShelfRepository : IGenericRepository<Shelf>
    {
        Task<PaginatedList<Shelf>> GetWithFilterAsync(string? storageCode, int pageNumber, int pageSize);
        Task<Shelf?> GetByCodeAsync(string shelfCode);
        Task DeleteAsync(string shelfCode);
        Task<IEnumerable<Shelf>> GetByStorageCodeAsync(string storageCode);
        Task<IEnumerable<Shelf>> GetAllShelvesByStorageCodeAsync(string storageCode);
    }
}
