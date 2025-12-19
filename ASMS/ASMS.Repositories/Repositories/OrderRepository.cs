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
        public async Task<IEnumerable<Order>> GetOverdueOrdersAsync(DateOnly currentDate)
        {
            return await _dbSet
                .Where(o => (o.Status == "stored" || o.Status == "renting" || o.Status.StartsWith("overdue"))
                    && o.ReturnDate.HasValue
                    && o.ReturnDate.Value < currentDate)
                .ToListAsync();
        }
        public async Task<IEnumerable<Order>> GetByStatusAsync(string status)
        {
            return await _dbSet
        .Where(o => o.Status != null && o.Status.ToLower() == status.ToLower())
        .AsNoTracking()
        .ToListAsync();
        }
        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            return await _dbSet
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<List<Order>> GetActiveOrdersByEmployeeAsync(string employeeCode)
        {
            try
            {
                var orders = await _dbSet
                    .Where(o => o.Status != null && o.Status.ToLower() != "retrieved")
                    .Where(o => _context.TrackingHistories
                        .Where(th => th.OrderCode == o.OrderCode)
                        .OrderByDescending(th => th.CreateAt)
                        .ThenByDescending(th => th.TrackingHistoryId)
                        .Select(th => th.CurrentAssign)
                        .FirstOrDefault() == employeeCode)
                    .Include(o => o.CustomerCodeNavigation)
                    .AsNoTracking()
                    .ToListAsync();

                return orders;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting active orders for employee {EmployeeCode}", employeeCode);
                throw;
            }
        }
        public async Task<int> GetNumberOfOrders(DateOnly startDate, DateOnly endDate, string? status)
        {
            var query = _dbSet
                .AsNoTracking()
                .Where(o => o.OrderDate.HasValue &&
                           o.OrderDate.Value >= startDate &&
                           o.OrderDate.Value <= endDate);

            if (!string.IsNullOrWhiteSpace(status))
            {
                string statusLower = status.ToLower();
                query = query.Where(o => o.Status != null && o.Status.ToLower() == statusLower);
            }

            return await query.CountAsync();
        }
        public IQueryable<Order> GetAllToCaculatePrice()
        {
            return _dbSet.AsQueryable();
        }

        public async Task<Order?> GetFullOrder(string orderCode)
        {
            var order = await _dbSet
                .Include(o => o.OrderDetails)
                    .ThenInclude(d => d.StorageCodeNavigation)
                        .ThenInclude(s => s.StorageType)
                .Include(o => o.OrderDetails)
                    .ThenInclude(d => d.ContainerCodeNavigation)
                        .ThenInclude(c => c.ContainerType)
                .Include(o => o.OrderDetails)
                    .ThenInclude(d => d.OrderDetailProductTypes)
                        .ThenInclude(pt => pt.ProductType)
                .Include(o => o.OrderDetails)
                    .ThenInclude(d => d.OrderDetailServices)
                        .ThenInclude(s => s.Service)
                .Include(o => o.CustomerCodeNavigation)
                .Include(o => o.PaymentHistories)
                .Include(o => o.TrackingHistories)
                .Include(o => o.PaymentResults)
                .FirstOrDefaultAsync(o => o.OrderCode == orderCode);

            if (order?.OrderDetails.Any() == true)
            {
                var containerTypeIds = order.OrderDetails
                    .Where(d => d.ContainerType.HasValue)
                    .Select(d => d.ContainerType!.Value)
                    .Distinct()
                    .ToList();

                var shelfTypeIds = order.OrderDetails
                    .Where(d => d.ShelfTypeId.HasValue)
                    .Select(d => d.ShelfTypeId!.Value)
                    .Distinct()
                    .ToList();

                var storageTypeIds = order.OrderDetails
                    .Where(d => d.StorageTypeId.HasValue)
                    .Select(d => d.StorageTypeId!.Value)
                    .Distinct()
                    .ToList();

                Dictionary<int, ContainerType> containerTypes = new();
                if (containerTypeIds.Any())
                {
                    containerTypes = await _context.ContainerTypes
                        .Where(ct => containerTypeIds.Contains(ct.ContainerTypeId))
                        .ToDictionaryAsync(ct => ct.ContainerTypeId);
                }

                Dictionary<int, ShelfType> shelfTypes = new();
                if (shelfTypeIds.Any())
                {
                    shelfTypes = await _context.ShelfTypes
                        .Where(st => shelfTypeIds.Contains(st.ShelfTypeId))
                        .ToDictionaryAsync(st => st.ShelfTypeId);
                }

                Dictionary<int, StorageType> storageTypes = new();
                if (storageTypeIds.Any())
                {
                    storageTypes = await _context.StorageTypes
                        .Where(st => storageTypeIds.Contains(st.StorageTypeId))
                        .ToDictionaryAsync(st => st.StorageTypeId);
                }

                foreach (var detail in order.OrderDetails)
                {
                    if (detail.ContainerType.HasValue &&
                        containerTypes.TryGetValue(detail.ContainerType.Value, out var containerType))
                    {
                        detail.ContainerTypeNavigation = containerType;
                    }

                    if (detail.ShelfTypeId.HasValue &&
                        shelfTypes.TryGetValue(detail.ShelfTypeId.Value, out var shelfType))
                    {
                        detail.ShelfTypeNavigation = shelfType;
                    }

                    if (detail.StorageTypeId.HasValue &&
                        storageTypes.TryGetValue(detail.StorageTypeId.Value, out var storageType))
                    {
                        detail.StorageTypeNavigation = storageType;
                    }
                }
            }

            return order;
        }
        public async Task<Order?> GetByPassKeyAsync(int passKey)
        {
            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Passkey == passKey && o.Status != "completed");
        }
        public async Task<List<Order>> GetByStatusStartsWithAsync(string statusPrefix)
        {
            return await _dbSet
                .Where(o => o.Status != null && o.Status.ToLower().StartsWith(statusPrefix.ToLower()))
                .ToListAsync();
        }

    }
}
