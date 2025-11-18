using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASMS.Repositories.Entities;
using ASMS.Repositories.Infrastructures;

namespace ASMS.Repositories.Interfaces
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        Task<List<Order>> GetWithFilterAsync(int pageNumber, int pageSize, string? customerCode, DateOnly? orderDate, DateOnly? depositDate, DateOnly? returnDate);
        Task<int> GetTotalCountWithFilterAsync(string? customerCode, DateOnly? orderDate, DateOnly? depositDate, DateOnly? returnDate);
        Task<Order?> GetByCodeAsync(string orderCode);
        Task<Order?> GetWithDetailsAsync(string orderCode);

        Task<int> CountOrdersByDateAsync(DateOnly date);
    }
}
