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
    public class ContainerLocationLogRepository : GenericRepository<ContainerLocationLog>, IContainerLocationLogRepository
    {
        public ContainerLocationLogRepository(VstorageContext context, ILogger logger) : base(context, logger)
        {
        }

        public async Task<IEnumerable<ContainerLocationLog>> GetAllAsync()
        {
            return await _dbSet.AsNoTracking().ToListAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
            }
        }
        public async Task<List<ContainerLocationLog>> GetByContainerCodeAsync(string containerCode)
        {
            return await _dbSet
                .Where(log => log.ContainerCode == containerCode)
                .OrderByDescending(log => log.UpdatedDate)
                .ToListAsync();
        }
        public async Task<ContainerLocationLog> GetLastAsync()
        {
            return await _context.ContainerLocationLogs
                .OrderByDescending(log => log.ContainerLocationLogId)
                .FirstOrDefaultAsync();
        }
    }
}
