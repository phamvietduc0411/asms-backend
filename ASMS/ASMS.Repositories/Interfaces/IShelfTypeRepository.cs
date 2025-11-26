using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASMS.Repositories.Common;
using ASMS.Repositories.Entities;

namespace ASMS.Repositories.Interfaces
{
    public interface IShelfTypeRepository
    {
        Task<PaginatedList<ShelfType>> GetAllAsync(int pageNumber, int pageSize);
        Task<ShelfType> GetByIdAsync(int id);
        Task AddAsync(ShelfType shelfType);
        Task UpdateAsync(ShelfType shelfType);
        Task DeleteAsync(ShelfType shelfType);
    }
}
