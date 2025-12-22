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
        Task<PaginatedList<OrderDetailItemResponse>> GetWithFilterAsync(bool? isPlaced, string? orderCode, string? storageCode, string? status, bool? isDamaged ,int pageNumber, int pageSize);
        Task<OrderDetailItemResponse?> GetByIdAsync(int id);
        Task<IEnumerable<OrderDetailItemResponse>> GetByOrderCodeAsync(string orderCode);
        Task<OrderDetailResponse> CreateAsync(CreateOrderDetailRequest request);
        Task<OrderDetailResponse?> UpdateAsync(int id, UpdateOrderDetailRequest request);
        //Task<bool> DeleteAsync(int id);
        Task<OrderDetailResponse?> UpdateStatusAsync(int orderDetailId, string status);
        Task<OrderDetailResponse?> UpdateIsDamagedAsync(int orderDetailId, bool isDamaged);
    }
}
