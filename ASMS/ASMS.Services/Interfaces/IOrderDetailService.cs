using ASMS.Services.Model.OrderDetail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Interfaces
{
    public interface IOrderDetailService
    {
        Task<IEnumerable<OrderDetailResponse>> GetAllAsync();
        Task<OrderDetailResponse?> GetByIdAsync(int id);
        Task<IEnumerable<OrderDetailResponse>> GetByOrderCodeAsync(string orderCode);
        Task<OrderDetailResponse> CreateAsync(CreateOrderDetailRequest request);
        Task<OrderDetailResponse?> UpdateAsync(int id, UpdateOrderDetailRequest request);
        //Task<bool> DeleteAsync(int id);
    }
}
