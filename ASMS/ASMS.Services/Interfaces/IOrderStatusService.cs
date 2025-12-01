using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASMS.Services.Model.OrderStatus;

namespace ASMS.Services.Interfaces
{
    public interface IOrderStatusService
    {
        /// <summary>
        /// Tự động kiểm tra và cập nhật trạng thái Overdue cho các order quá hạn
        /// </summary>
        Task CheckAndUpdateOverdueOrdersAsync();

        /// <summary>
        /// Cập nhật trạng thái Order tự động theo quy trình của từng Style
        /// </summary>
        /// <param name="orderCode">Mã đơn hàng</param>
        Task UpdateOrderStatusAsync(string orderCode);

        /// <summary>
        /// Gia hạn đơn hàng
        /// </summary>
        /// <param name="orderCode">Mã đơn hàng</param>
        /// <param name="newReturnDate">Ngày trả mới</param>
        Task<OrderStatusResponse?> ExtendOrderAsync(string orderCode, DateOnly newReturnDate);

        /// <summary>
        /// Chuyển đơn hàng vào kho quá hạn
        /// </summary>
        /// <param name="orderCode">Mã đơn hàng</param>
        Task<bool> MoveToExpiredStorageAsync(string orderCode);

        /// <summary>
        /// Cập nhật trạng thái thanh toán (chuyển sang/hoặc thoát khỏi Waiting for Payment)
        /// </summary>
        /// <param name="orderCode">Mã đơn hàng</param>
        Task<OrderStatusResponse?> TogglePaymentStatusAsync(string orderCode);

        /// <summary>
        /// Lấy thông tin trạng thái hiện tại của Order
        /// </summary>
        /// <param name="orderCode">Mã đơn hàng</param>
        Task<OrderStatusResponse?> GetOrderStatusAsync(string orderCode);
    }
}
