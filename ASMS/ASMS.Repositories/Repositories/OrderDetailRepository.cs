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
    public class OrderDetailRepository : GenericRepository<OrderDetail>, IOrderDetailRepository
    {
        public OrderDetailRepository(VstorageContext context, ILogger logger) : base(context, logger)
        {
        }

        public async Task<PaginatedList<OrderDetail>> GetWithFilterAsync(bool? isPlaced, string? orderCode, string? storageCode, int pageNumber, int pageSize)
        {
            var query = _context.OrderDetails
                .Include(x => x.OrderCodeNavigation)
                .Include(x => x.StorageCodeNavigation)
                .Include(x => x.ContainerCodeNavigation)
                .AsQueryable();

            if (isPlaced.HasValue)
            {
                query = query.Where(od => od.IsPlaced == isPlaced.Value);
            }

            if (!string.IsNullOrEmpty(orderCode))
            {
                query = query.Where(od => od.OrderCode == orderCode);
            }
            if (!string.IsNullOrEmpty(storageCode))
            {
                query = query.Where(od => od.StorageCode == storageCode);
            }

            query = query.OrderBy(od => od.OrderDetailId);

            return await PaginatedList<OrderDetail>.CreateAsync(query, pageNumber, pageSize);
        }

        public async Task<OrderDetail?> GetByIdAsync(int id)
        {
            return await _context.OrderDetails
                .Include(x => x.OrderCodeNavigation)
                .Include(x => x.StorageCodeNavigation)
                .Include(x => x.ContainerCodeNavigation)
                .FirstOrDefaultAsync(x => x.OrderDetailId == id);
        }

        public async Task<List<OrderDetail>> GetByOrderCodeAsync(string orderCode)
        {
            return await _context.OrderDetails
                .Include(od => od.OrderDetailProductTypes)
                    .ThenInclude(odpt => odpt.ProductType)
                .Include(od => od.OrderDetailServices)
                    .ThenInclude(ods => ods.Service)
                .Include(od => od.ContainerCodeNavigation)
                .AsNoTracking()
                //.Include (x => x.OrderCodeNavigation)
                .Where(x => x.OrderCode == orderCode)
                .ToListAsync();
        }

        public async Task<int> GetMaxOrderDetailIdAsync()
        {
            if (!await _dbSet.AnyAsync())
                return 0;

            return await _dbSet.AsNoTracking().MaxAsync(od => od.OrderDetailId);
        }
    }
}
