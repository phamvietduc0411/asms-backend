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
    public class ContainerLocationLogRepository
        : GenericRepository<ContainerLocationLog>, IContainerLocationLogRepository
    {
        public ContainerLocationLogRepository(VstorageContext context, ILogger logger)
            : base(context, logger)
        {
        }

        public async Task<IEnumerable<ContainerLocationLog>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<ContainerLocationLog?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<ContainerLocationLog>> GetByContainerCodeAsync(string containerCode)
        {
            return await _dbSet.Where(x => x.ContainerCode == containerCode).ToListAsync();
        }

        public async Task<IEnumerable<ContainerLocationLog>> GetByOrderCodeAsync(string orderCode)
        {
            return await _dbSet.Where(x => x.OrderCode == orderCode).ToListAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity == null) return false;

            _dbSet.Remove(entity);
            return true;
        }
    }

}
