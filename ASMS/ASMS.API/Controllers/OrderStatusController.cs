using ASMS.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderStatusController : ControllerBase
    {
        private readonly IOrderStatusService _orderStatusService;

        public OrderStatusController(IOrderStatusService orderStatusService)
        {
            _orderStatusService = orderStatusService;
        }

        /// <summary>
        /// Kiểm tra và cập nhật tất cả đơn hàng quá hạn
        /// </summary>
        /// <returns></returns>
        [HttpPost("check-overdue")]
        public async Task<IActionResult> CheckOverdueOrders()
        {
            try
            {
                await _orderStatusService.CheckAndUpdateOverdueOrdersAsync();
                return Ok(new { message = "Overdue orders checked and updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Cập nhật trạng thái Order sang trạng thái tiếp theo theo quy trình
        /// </summary>
        /// <param name="orderCode">Mã đơn hàng</param>
        /// <returns></returns>
        [HttpPost("{orderCode}/update-status")]
        public async Task<IActionResult> UpdateOrderStatus(string orderCode)
        {
            try
            {
                await _orderStatusService.UpdateOrderStatusAsync(orderCode);
                var result = await _orderStatusService.GetOrderStatusAsync(orderCode);

                if (result == null)
                {
                    return NotFound(new { message = $"Order {orderCode} not found" });
                }

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Gia hạn đơn hàng
        /// </summary>
        /// <param name="orderCode">Mã đơn hàng</param>
        /// <param name="newReturnDate">Ngày trả mới (YYYY-MM-DD)</param>
        /// <returns></returns>
        [HttpPost("{orderCode}/extend")]
        public async Task<IActionResult> ExtendOrder(string orderCode, [FromQuery] string newReturnDate)
        {
            try
            {
                if (!DateOnly.TryParse(newReturnDate, out var returnDate))
                {
                    return BadRequest(new { error = "Invalid date format. Use YYYY-MM-DD" });
                }

                var result = await _orderStatusService.ExtendOrderAsync(orderCode, returnDate);

                if (result == null)
                {
                    return NotFound(new { message = $"Order {orderCode} not found" });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Chuyển đơn hàng vào kho quá hạn
        /// </summary>
        /// <param name="orderCode">Mã đơn hàng</param>
        /// <returns></returns>
        [HttpPost("{orderCode}/move-to-expired-storage")]
        public async Task<IActionResult> MoveToExpiredStorage(string orderCode)
        {
            try
            {
                var success = await _orderStatusService.MoveToExpiredStorageAsync(orderCode);

                if (!success)
                {
                    return NotFound(new { message = $"Order {orderCode} not found" });
                }

                return Ok(new { message = $"Order {orderCode} moved to expired storage successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Toggle trạng thái thanh toán (chuyển sang/ra khỏi Waiting for Payment)
        /// </summary>
        /// <param name="orderCode">Mã đơn hàng</param>
        /// <returns></returns>
        [HttpPost("{orderCode}/toggle-payment")]
        public async Task<IActionResult> TogglePaymentStatus(string orderCode)
        {
            try
            {
                var result = await _orderStatusService.TogglePaymentStatusAsync(orderCode);

                if (result == null)
                {
                    return NotFound(new { message = $"Order {orderCode} not found" });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// Lấy thông tin trạng thái hiện tại của Order
        /// </summary>
        /// <param name="orderCode">Mã đơn hàng</param>
        /// <returns></returns>
        [HttpGet("{orderCode}/status")]
        public async Task<IActionResult> GetOrderStatus(string orderCode)
        {
            try
            {
                var result = await _orderStatusService.GetOrderStatusAsync(orderCode);

                if (result == null)
                {
                    return NotFound(new { message = $"Order {orderCode} not found" });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
