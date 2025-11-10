using ASMS.Repositories.Common;
using ASMS.Repositories.Data;
using ASMS.Repositories.Entities;
using ASMS.Repositories.Infrastructures;
using ASMS.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Repositories.Repositories
{
    public class ShelfRepository : GenericRepository<Shelf>, IShelfRepository
    {
        public ShelfRepository(VstorageContext context, ILogger logger)
            : base(context, logger)
        {
        }

        public async Task<PaginatedList<Shelf>> GetWithFilterAsync(string? storageCode, int pageNumber, int pageSize)
        {
            var query = _context.Shelves.AsQueryable();

            if (!string.IsNullOrEmpty(storageCode))
            {
                query = query.Where(s => s.StorageCode == storageCode);
            }

            query = query.OrderBy(s => s.ShelfCode);

            return await PaginatedList<Shelf>.CreateAsync(query, pageNumber, pageSize);
        }

        public async Task<Shelf?> GetByCodeAsync(string shelfCode)
        {
            return await _dbSet.Include(s => s.StorageCodeNavigation)
                               .FirstOrDefaultAsync(s => s.ShelfCode == shelfCode);
        }

        public async Task DeleteAsync(string shelfCode)
        {
            var entity = await _dbSet.FirstOrDefaultAsync(s => s.ShelfCode == shelfCode);
            if (entity != null)
            {
                _dbSet.Remove(entity);
            }
        }
        public async Task<IEnumerable<Shelf>> GetByStorageCodeAsync(string storageCode)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(s => s.StorageCode == storageCode)
                .ToListAsync();
        }
    }
}
