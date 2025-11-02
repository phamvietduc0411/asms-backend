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
    public class OrderDetailRepository : GenericRepository<OrderDetail>, IOrderDetailRepository
    {
        public OrderDetailRepository(VstorageContext context, ILogger logger) : base(context, logger)
        {
        }

        public async Task<IEnumerable<OrderDetail>> GetAllAsync()
        {
            return await _context.OrderDetails
                .Include(x => x.OrderCodeNavigation)
                .Include(x => x.StorageCodeNavigation)
                .Include(x => x.ContainerCodeNavigation)
                .Include(x => x.Service)
                .ToListAsync();
        }

        public async Task<OrderDetail?> GetByIdAsync(int id)
        {
            return await _context.OrderDetails
                .Include(x => x.OrderCodeNavigation)
                .Include(x => x.StorageCodeNavigation)
                .Include(x => x.ContainerCodeNavigation)
                .Include(x => x.Service)
                .FirstOrDefaultAsync(x => x.OrderDetailId == id);
        }

        public async Task<List<OrderDetail>> GetByOrderCodeAsync(string orderCode)
        {
            return await _context.OrderDetails
                .Where(x => x.OrderCode == orderCode)
                .ToListAsync();
        }
    }
}
