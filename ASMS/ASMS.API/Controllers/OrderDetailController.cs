using ASMS.Services.Interfaces;
using ASMS.Services.Model.OrderDetail;
using Microsoft.AspNetCore.Mvc;

namespace ASMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderDetailController : ControllerBase
    {
        private readonly IOrderDetailService _service;

        public OrderDetailController(IOrderDetailService service)
        {
            _service = service;
        }

        /// <summary>
        /// Retrieves all order details with optional filters and pagination
        /// </summary>
        /// <param name="isPlaced">Optional filter by placed status (true/false/null)</param>
        /// <param name="orderCode">Optional filter by order code</param>
        /// <param name="storageCode"></param>
        /// <param name="status"></param>
        /// <param name="isDamaged"></param>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10, max: 100)</param>
        /// <returns>List of order details</returns>
        [HttpGet]
        public async Task<IActionResult> GetOrderDetails(
            [FromQuery] bool? isPlaced,
            [FromQuery] string? orderCode,
            [FromQuery] string? storageCode,
            [FromQuery] string? status,
            [FromQuery] bool? isDamaged,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {

                var result = await _service.GetWithFilterAsync(isPlaced, orderCode, storageCode, status, isDamaged, pageNumber, pageSize);

                return Ok(new
                {
                    success = true,
                    data = result.Items,
                    pagination = new
                    {
                        currentPage = result.CurrentPage,
                        pageSize = result.PageSize,
                        totalRecords = result.TotalRecords,
                        totalPages = result.TotalPages
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpGet("by-order/{orderCode}")]
        public async Task<IActionResult> GetByOrderCode(string orderCode)
        {
            var result = await _service.GetByOrderCodeAsync(orderCode);
            return Ok(result);
        }

        //[HttpPost]
        //public async Task<IActionResult> Create([FromBody] CreateOrderDetailRequest request)
        //{
        //    var result = await _service.CreateAsync(request);
        //    return Ok(result);
        //}

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateOrderDetailRequest request)
        {
            var result = await _service.UpdateAsync(id, request);
            return result == null ? NotFound() : Ok(result);
        }

        //[HttpDelete("{id}")]
        //public async Task<IActionResult> Delete(int id)
        //{
        //    var success = await _service.DeleteAsync(id);
        //    return success ? Ok() : NotFound();
        //}
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusRequest request)
        {
            var result = await _service.UpdateStatusAsync(id, request.Status);
            if (result == null)
                return NotFound(new { message = $"OrderDetail {id} not found" });

            return Ok(result);
        }

        [HttpPatch("{id}/damage")]
        public async Task<IActionResult> UpdateIsDamaged(int id, [FromBody] UpdateIsDamagedRequest request)
        {
            var result = await _service.UpdateIsDamagedAsync(id, request.IsDamaged);
            if (result == null)
                return NotFound(new { message = $"OrderDetail {id} not found" });

            return Ok(result);
        }

    }

}
