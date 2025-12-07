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
        public async Task<IEnumerable<OrderDetailService>> GetByOrderDetailIdAsync(int orderDetailId)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(ods => ods.Service)
                .Where(ods => ods.OrderDetailId == orderDetailId)
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<List<OrderDetailService>> GetByOrderDetailIdForDeleteAsync(int orderDetailId)
        {
            return await _dbSet
                .Where(ods => ods.OrderDetailId == orderDetailId)
                .ToListAsync();
        }
        public async Task DeleteByOrderDetailIdAsync(int orderDetailId)
        {
            await _context.Database.ExecuteSqlRawAsync(
                "DELETE FROM OrderDetailService WHERE OrderDetailId = {0}",
                orderDetailId);
        }
    }
}
