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
    public class FloorBlockRepository : GenericRepository<FloorBlock>, IFloorBlockRepository
    {
        public FloorBlockRepository(VstorageContext context, ILogger logger)
            : base(context, logger)
        {
        }

        public async Task<IEnumerable<FloorBlock>> GetAllAsync()
        {
            return await _dbSet.Include(f => f.FloorCodeNavigation)
                               .AsNoTracking()
                               .ToListAsync();
        }

        public async Task<FloorBlock?> GetByCodeAsync(string floorBlockCode)
        {
            return await _dbSet.Include(f => f.FloorCodeNavigation)
                               .FirstOrDefaultAsync(x => x.FloorBlockCode == floorBlockCode);
        }

        public async Task DeleteAsync(string floorBlockCode)
        {
            var entity = await _dbSet.FirstOrDefaultAsync(x => x.FloorBlockCode == floorBlockCode);
            if (entity != null)
            {
                _dbSet.Remove(entity);
            }
        }
    }

}
