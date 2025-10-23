using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASMS.Repositories.Infrastructures;
using ASMS.Services.Model.Orders;

namespace ASMS.Services.Interfaces
{
    public interface IOrderService
    {
        Task<PaginatedOrderResponse> GetWithFilterAsync(int pageNumber, int pageSize, string? customerCode, DateOnly? orderDate, DateOnly? depositDate, DateOnly? returnDate);
        Task<OrderResponse?> GetByCodeAsync(string orderCode);
        Task<OrderResponse> CreateAsync(CreateOrderRequest request);
        Task<OrderResponse> UpdateAsync(string orderCode, UpdateOrderRequest request);
    }
}
