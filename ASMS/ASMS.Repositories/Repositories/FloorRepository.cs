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
    public class FloorRepository : GenericRepository<Floor>, IFloorRepository
    {
        public FloorRepository(VstorageContext context, ILogger logger) : base(context, logger) { }

        public async Task<IEnumerable<Floor>> GetAllAsync()
        {
            return await _dbSet.AsNoTracking().ToListAsync();
        }

        public async Task<Floor?> GetByCodeAsync(string floorCode)
        {
            return await _dbSet
                .Include(f => f.ShelfCodeNavigation)
                .FirstOrDefaultAsync(f => f.FloorCode == floorCode);
        }

        public async Task DeleteAsync(Floor entity)
        {
            _dbSet.Remove(entity);
            await Task.CompletedTask;
        }
        public async Task<List<Floor>> GetByShelfCodeAsync(string shelfCode)
        {
            return await _dbSet
                .Where(f => f.ShelfCode == shelfCode && f.IsActive == true)
                .OrderBy(f => f.FloorNumber)
                .ToListAsync(); 
        }
        public async Task<List<Floor>> GetByFloorNumbersAsync(List<int> floorNumbers)
        {
            return await _dbSet
                .Where(f => floorNumbers.Contains(f.FloorNumber.GetValueOrDefault())
                    && f.IsActive == true)
                .OrderBy(f => f.FloorNumber)
                .ToListAsync();
        }

    }
}
