using ASMS.Repositories.Data;
using ASMS.Repositories.Entities;
using ASMS.Repositories.Infrastructures;
using ASMS.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Repositories.Repositories
{
    public class OrderDetailServiceRepository : GenericRepository<OrderDetailService>, IOrderDetailServiceRepository
    {
        public OrderDetailServiceRepository(
           VstorageContext context, ILogger logger) : base(context, logger)
        {
        }
        public async Task<List<Service>> GetByIdsAsync(List<int> ids)
        {
            if (ids.IsNullOrEmpty()) return [];
            return await _context.Services
                .Where(s => ids.Contains(s.ServiceId))
                .ToListAsync();
        }
    }
}
