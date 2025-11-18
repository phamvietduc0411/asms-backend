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
    public class ContainerRepository : GenericRepository<Container>, IContainerRepository
    {
        public ContainerRepository(VstorageContext context, ILogger logger) : base(context, logger)
        {
        }

        public async Task<PaginatedList<Container>> GetAllAsync(int pageNumber, int pageSize)
        {
            var query = _context.Containers
                .Include(c => c.ContainerType)
                .OrderBy(c => c.ContainerCode);

            return await PaginatedList<Container>.CreateAsync(query, pageNumber, pageSize);
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
        public async Task UpdateStackingInfoAsync(string containerCode, int layer, int serialNumber, string containerAboveCode)
        {
            var container = await _context.Containers.FirstOrDefaultAsync(c => c.ContainerCode == containerCode);
            if(container != null)
            {
                container.Layer = layer;
                container.SerialNumber = serialNumber;
                container.ContainerAboveCode = containerAboveCode;
                await _context.SaveChangesAsync();
            }
        }
        public async Task MoveContainerToLayer1Async(string containerCode)
        {
            var container = await _context.Containers
                .FirstOrDefaultAsync(c => c.ContainerCode == containerCode);
            if (container != null)
            {
                container.Layer = 1;
                await _context.SaveChangesAsync();
            }
        }

    }
}
