using ASMS.Services.Interfaces;
using ASMS.Services.Model.PaymentHistory;
using Microsoft.AspNetCore.Mvc;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace ASMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentHistoryController : ControllerBase
    {
        private readonly IPaymentHistoryService _service;

        public PaymentHistoryController(IPaymentHistoryService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{code}")]
        public async Task<IActionResult> GetByCode(string code)
        {
            var result = await _service.GetByCodeAsync(code);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePaymentHistoryRequest request)
        {
            var result = await _service.CreateAsync(request);
            return CreatedAtAction(nameof(GetByCode), new { code = result.PaymentHistoryCode }, result);
        }

        [HttpPut("{code}")]
        public async Task<IActionResult> Update(string code, [FromBody] UpdatePaymentHistoryRequest request)
        {
            var result = await _service.UpdateAsync(code, request);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpDelete("{code}")]
        public async Task<IActionResult> Delete(string code)
        {
            var success = await _service.DeleteAsync(code);
            if (!success) return NotFound();
            return NoContent();
        }

        [HttpGet("by-customerCode")]
        public async Task<IActionResult> GetPaymentHistoryByCustomerCode([FromQuery] string? customerCode, [FromQuery] string? orderCode)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(customerCode))
                    return BadRequest(new { success = false, message = "CustomerCode is required." });

                var history = await _service.GetHistory(customerCode, orderCode);

                return Ok(new
                {
                    success = true,
                    data = history
                });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }


    }
}
