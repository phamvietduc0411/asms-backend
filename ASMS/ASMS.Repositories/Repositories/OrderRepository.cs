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
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        public OrderRepository(VstorageContext context, ILogger logger) : base(context, logger) { }
        public async Task<List<Order>> GetWithFilterAsync(int pageNumber, int pageSize, string? customerCode, DateOnly? orderDate, DateOnly? depositDate, DateOnly? returnDate, string style)
        {
            try
            {
                var query = _dbSet
                    .Include(o => o.CustomerCodeNavigation)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(customerCode))
                    query = query.Where(o => o.CustomerCode == customerCode);

                if (orderDate.HasValue)
                    query = query.Where(o => o.OrderDate == orderDate.Value);

                if (depositDate.HasValue)
                    query = query.Where(o => o.DepositDate == depositDate.Value);

                if (returnDate.HasValue)
                    query = query.Where(o => o.ReturnDate == returnDate.Value);

                if (!string.IsNullOrWhiteSpace(style))
                    query = query.Where(o => o.Style == style);

                return await query
                    .OrderByDescending(o => o.OrderDate)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting orders with filter");
                throw;
            }
        }

        public async Task<int> GetTotalCountWithFilterAsync(string? customerCode, DateOnly? orderDate, DateOnly? depositDate, DateOnly? returnDate, string style)
        {
            try
            {
                var query = _dbSet.AsQueryable();

                if (!string.IsNullOrWhiteSpace(customerCode))
                    query = query.Where(o => o.CustomerCode == customerCode);

                if (orderDate.HasValue)
                    query = query.Where(o => o.OrderDate == orderDate.Value);

                if (depositDate.HasValue)
                    query = query.Where(o => o.DepositDate == depositDate.Value);

                if (returnDate.HasValue)
                    query = query.Where(o => o.ReturnDate == returnDate.Value);

                if (!string.IsNullOrWhiteSpace(style))
                    query = query.Where(o => o.Style == style);

                return await query.CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting orders");
                throw;
            }
        }

        public async Task<Order?> GetByCodeAsync(string orderCode)
        {
            try
            {
                return await _dbSet
                    .FirstOrDefaultAsync(o => o.OrderCode == orderCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting order by code: {Code}", orderCode);
                throw;
            }
        }
        public async Task<Order?> GetWithDetailsAsync(string orderCode)
        {
            return await _dbSet
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.ContainerCodeNavigation)
                .FirstOrDefaultAsync(o => o.OrderCode == orderCode);
        }
        public async Task<int> CountOrdersByDateAsync(DateOnly date)
        {
            return await _dbSet
                .Where(o => o.OrderDate == date)
                .CountAsync();
        }
    }
}
