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
    public class ContainerTypeRepository : GenericRepository<ContainerType>, IContainerTypeRepository
    {
        public ContainerTypeRepository(VstorageContext context, ILogger logger) : base(context, logger)
        {
        }
        public async Task<ContainerType?> GetByIdAsync(int containerTypeId)
        {
            return await _dbSet.FirstOrDefaultAsync(ct => ct.ContainerTypeId == containerTypeId);
        }
        public async Task<List<ContainerType>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }
    }
}
