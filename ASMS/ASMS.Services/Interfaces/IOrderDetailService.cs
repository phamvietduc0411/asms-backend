using ASMS.Repositories.Common;
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
        Task<PaginatedList<OrderDetailItemResponse>> GetWithFilterAsync(bool? isPlaced, string? orderCode, string? storageCode, int pageNumber, int pageSize);
        Task<OrderDetailResponse?> GetByIdAsync(int id);
        Task<IEnumerable<OrderDetailResponse>> GetByOrderCodeAsync(string orderCode);
        Task<OrderDetailResponse> CreateAsync(CreateOrderDetailRequest request);
        Task<OrderDetailResponse?> UpdateAsync(int id, UpdateOrderDetailRequest request);
        //Task<bool> DeleteAsync(int id);
    }
}
