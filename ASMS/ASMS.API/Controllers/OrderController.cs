using ASMS.Services.Interfaces;
using ASMS.Services.Model.Orders;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        #region Get Orders with Filter
        /// <summary>
        /// Get orders with pagination and optional filters.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetWithFilterAsync(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? customerCode = null,
            [FromQuery] DateOnly? orderDate = null,
            [FromQuery] DateOnly? depositDate = null,
            [FromQuery] DateOnly? returnDate = null)
        {
            if (pageNumber < 1 || pageSize < 1)
                return BadRequest("Page number and page size must be greater than 0.");

            var result = await _orderService.GetWithFilterAsync(pageNumber, pageSize, customerCode, orderDate, depositDate, returnDate);
            return Ok(result);
        }
        #endregion

        #region Get Order by Code
        /// <summary>
        /// Get an order by its code.
        /// </summary>
        [HttpGet("{orderCode}")]
        public async Task<IActionResult> GetByCodeAsync(string orderCode)
        {
            var result = await _orderService.GetByCodeAsync(orderCode);
            if (result == null)
                return NotFound($"Order with code '{orderCode}' not found.");
            return Ok(result);
        }
        #endregion

        #region Create Order
        /// <summary>
        /// Create a new order.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateOrderRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _orderService.CreateAsync(request);
                return Created($"/api/Order/{result.OrderCode}", result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        #endregion

        #region Update Order
        /// <summary>
        /// Update an existing order.
        /// </summary>
        [HttpPut("{orderCode}")]
        public async Task<IActionResult> UpdateAsync(string orderCode, [FromBody] UpdateOrderRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _orderService.UpdateAsync(orderCode, request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
        #endregion
    }
}
