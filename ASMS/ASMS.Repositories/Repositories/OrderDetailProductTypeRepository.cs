using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASMS.Repositories.Data;
using ASMS.Repositories.Entities;
using ASMS.Repositories.Infrastructures;
using ASMS.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ASMS.Repositories.Repositories
{
    public class OrderDetailProductTypeRepository : GenericRepository<OrderDetailProductType>, IOrderDetailProductTypeRepository
    {
        public OrderDetailProductTypeRepository(
           VstorageContext context, ILogger logger) : base(context, logger)
        {
        }
        public async Task<List<OrderDetailProductType>> GetByOrderDetailIdAsync(int orderDetailId)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(odpt => odpt.OrderDetailId == orderDetailId)
                .ToListAsync();
        }
        public async Task<List<OrderDetailProductType>> GetByOrderDetailIdForDeleteAsync(int orderDetailId)
        {
            return await _dbSet
                .Where(odpt => odpt.OrderDetailId == orderDetailId)
                .ToListAsync();
        }
        public async Task DeleteByOrderDetailIdAsync(int orderDetailId)
        {
            await _context.Database.ExecuteSqlRawAsync(
                "DELETE FROM OrderDetailProductType WHERE OrderDetailId = {0}",
                orderDetailId);
        }
    }
}
