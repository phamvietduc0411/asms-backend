using ASMS.Services.Interfaces;
using ASMS.Services.Model.OrderStatus;
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
        /// <param name="unpaidAmount"></param>
        /// <returns></returns>
        [HttpPost("{orderCode}/extend")]
        public async Task<IActionResult> ExtendOrder(string orderCode, [FromQuery] string newReturnDate, [FromQuery] decimal unpaidAmount)
        {
            try
            {
                if (!DateOnly.TryParse(newReturnDate, out var returnDate))
                {
                    return BadRequest(new { error = "Invalid date format. Use YYYY-MM-DD" });
                }

                var result = await _orderStatusService.ExtendOrderAsync(orderCode, returnDate, unpaidAmount);

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
        /// <summary>
        /// Cập nhật hình ảnh cho tracking history mới nhất của đơn hàng
        /// </summary>
        /// <param name="request">Thông tin order code và danh sách image URLs</param>
        /// <returns>Tracking history đã được cập nhật</returns>
        [HttpPut("tracking/update-image")]
        public async Task<IActionResult> UpdateLatestTrackingImage([FromBody] UpdateTrackingImageRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.OrderCode))
                {
                    return BadRequest(new { message = "OrderCode is required" });
                }

                var result = await _orderStatusService.UpdateLatestTrackingImageAsync(request);

                if (result == null)
                {
                    return NotFound(new { message = $"No tracking history found for order {request.OrderCode}" });
                }

                return Ok(new
                {
                    message = "Tracking image updated successfully",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating tracking image", error = ex.Message });
            }
        }
        [HttpPut("update-passkey")]
        public async Task<IActionResult> UpdatePassKey([FromBody] UpdatePassKeyRequest request)
        {
            var result = await _orderStatusService.UpdatePassKeyAsync(request);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPut("update-refund")]
        public async Task<IActionResult> UpdateRefund([FromBody] UpdateRefundRequest request)
        {
            var result = await _orderStatusService.UpdateRefundAsync(request);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        /// <summary>
        /// Hủy đơn hàng (chỉ cho phép khi status = pending)
        /// </summary>
        [HttpPost("cancel")]
        public async Task<IActionResult> CancelOrder([FromBody] CancelOrderRequest request)
        {
            var result = await _orderStatusService.CancelOrderAsync(request);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
