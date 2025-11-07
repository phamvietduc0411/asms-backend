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
    public class ContainerRepository : GenericRepository<Container>, IContainerRepository
    {
        public ContainerRepository(VstorageContext context, ILogger logger) : base(context, logger)
        {
        }

        public async Task<IEnumerable<Container>> GetAllAsync()
        {
            return await _dbSet
                .Include(c => c.FloorCodeNavigation)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Container?> GetByCodeAsync(string containerCode)
        {
            return await _dbSet
                .Include(c => c.ContainerType)
                .FirstOrDefaultAsync(c => c.ContainerCode == containerCode);
        }

        public async Task DeleteAsync(string code)
        {
            var entity = await _dbSet.FirstOrDefaultAsync(c => c.ContainerCode == code);
            if (entity != null)
            {
                _dbSet.Remove(entity);
            }
        }
        //public async Task<IEnumerable<Container>> GetByFloorCodeAsync(string floorCode)
        //{
        //    return await _dbSet
        //        .AsNoTracking()
        //        .Where(c => c.FloorCode == floorCode)
        //        .Include(c => c.ProductType)
        //        .ToListAsync();
        //}

        public async Task<List<Container>> GetAvailableByTypeAsync(int containerTypeId)
        {
            return await _dbSet
                .Where(c => c.ContainerTypeId == containerTypeId
                    && c.Status == "Available"
                    && c.FloorCode == null
                    && c.IsActive == true)
                .ToListAsync();
        }

        public async Task<List<Container>> GetByFloorCodeAsync(string floorCode)
        {
            return await _dbSet
                .Where(c => c.FloorCode == floorCode && c.IsActive == true)
                .OrderBy(c => c.PositionX)
                .ToListAsync();
        }

        public new async Task UpdateAsync(Container container)
        {
            _dbSet.Update(container);
            await Task.CompletedTask;
        }
       
    }
}
