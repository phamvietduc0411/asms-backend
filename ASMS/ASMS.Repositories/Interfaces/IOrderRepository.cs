using ASMS.Repositories.Entities;
using ASMS.Repositories.Infrastructures;

namespace ASMS.Repositories.Interfaces
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        Task<List<Order>> GetWithFilterAsync(int pageNumber, int pageSize, string? customerCode, DateOnly? orderDate, DateOnly? depositDate, DateOnly? returnDate, string style);
        Task<int> GetTotalCountWithFilterAsync(string? customerCode, DateOnly? orderDate, DateOnly? depositDate, DateOnly? returnDate, string style);
        Task<Order?> GetByCodeAsync(string orderCode);
        Task<Order?> GetWithDetailsAsync(string orderCode);
        Task<int> CountOrdersByDateAsync(DateOnly date);
        Task<IEnumerable<Order>> GetOverdueOrdersAsync(DateOnly currentDate);
        Task<IEnumerable<Order>> GetByStatusAsync(string status);
        Task<IEnumerable<Order>> GetAllAsync();
        Task<List<Order>> GetActiveOrdersByEmployeeAsync(string employeeCode);
        Task<int> GetNumberOfOrders(DateOnly startDate, DateOnly endDate, string? status);
        IQueryable<Order> GetAllToCaculatePrice();
        Task<Order?> GetFullOrder(string orderCode);
        Task<Order?> GetByPassKeyAsync(int passKey);
    }
}
