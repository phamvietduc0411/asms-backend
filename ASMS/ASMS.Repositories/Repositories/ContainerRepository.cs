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

        public async Task<Container?> GetByCodeAsync(string code)
        {
            return await _dbSet
                .Include(c => c.FloorCodeNavigation)
                .FirstOrDefaultAsync(c => c.ContainerCode == code);
        }

        public async Task DeleteAsync(string code)
        {
            var entity = await _dbSet.FirstOrDefaultAsync(c => c.ContainerCode == code);
            if (entity != null)
            {
                _dbSet.Remove(entity);
            }
        }
    }
}
