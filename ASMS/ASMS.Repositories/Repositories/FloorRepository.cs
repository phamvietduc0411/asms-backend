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
    public class FloorRepository : GenericRepository<Floor>, IFloorRepository
    {
        public FloorRepository(VstorageContext context, ILogger logger) : base(context, logger) { }

        public async Task<PaginatedList<Floor>> GetWithFilterAsync(string? shelfCode, int pageNumber, int pageSize)
        {
            var query = _context.Floors.AsQueryable();

            if (!string.IsNullOrEmpty(shelfCode))
            {
                query = query.Where(f => f.ShelfCode == shelfCode);
            }

            query = query.OrderBy(f => f.FloorCode);

            return await PaginatedList<Floor>.CreateAsync(query, pageNumber, pageSize);
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
            return await _context.Floors
                .Include(f => f.ShelfCodeNavigation)                    
                    .ThenInclude(s => s.StorageCodeNavigation)         
                        .ThenInclude(st => st.Building)                 
                .Where(f => floorNumbers.Contains(f.FloorNumber.Value))
                .ToListAsync();
        }
        public async Task<List<Floor>> GetFloorsByBuildingAndNumberAsync(int buildingId, List<int> floorNumbers)
        {
            return await _context.Floors
                .Include(f => f.ShelfCodeNavigation)
                    .ThenInclude(s => s.StorageCodeNavigation)
                .Where(f => f.ShelfCodeNavigation.StorageCodeNavigation.BuildingId == buildingId
                        && floorNumbers.Contains(f.FloorNumber.Value))
                .OrderBy(f => f.FloorNumber)
                .ToListAsync();
        }

    }
}
