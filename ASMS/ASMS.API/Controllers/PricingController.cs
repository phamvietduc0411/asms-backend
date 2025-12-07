using ASMS.Services.Interfaces;
using ASMS.Services.Model.Pricing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PricingController : ControllerBase
    {
        private readonly IPricingService _pricingService;

        public PricingController(IPricingService pricingService)
        {
            _pricingService = pricingService;
        }

        // ==================== PRICING ENDPOINTS ====================

        /// <summary>
        /// Get all pricing records
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllPricing()
        {
            var result = await _pricingService.GetAllPricingAsync();
            return Ok(result);
        }

        /// <summary>
        /// Get pricing by ID
        /// </summary>
        [HttpGet("{pricingId}")]
        public async Task<IActionResult> GetPricingById(int pricingId)
        {
            var result = await _pricingService.GetPricingByIdAsync(pricingId);
            if (result == null)
                return NotFound(new { message = "Pricing not found" });
            return Ok(result);
        }

        /// <summary>
        /// Get pricing by service type (RoomRental, ShelfRental, BoxSelfManaged, BoxShared)
        /// </summary>
        [HttpGet("service-type/{serviceType}")]
        public async Task<IActionResult> GetPricingByServiceType(string serviceType)
        {
            var result = await _pricingService.GetPricingByServiceTypeAsync(serviceType);
            return Ok(result);
        }

        /// <summary>
        /// Create new pricing record
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreatePricing([FromBody] CreatePricingRequest request)
        {
            try
            {
                var result = await _pricingService.CreatePricingAsync(request);
                return CreatedAtAction(nameof(GetPricingById), new { pricingId = result.PricingId }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Update existing pricing record
        /// </summary>
        [HttpPut("{pricingId}")]
        public async Task<IActionResult> UpdatePricing(int pricingId, [FromBody] UpdatePricingRequest request)
        {
            request.PricingId = pricingId;
            var result = await _pricingService.UpdatePricingAsync(request);
            if (result == null)
                return NotFound(new { message = "Pricing not found" });
            return Ok(result);
        }

        /// <summary>
        /// Delete pricing record
        /// </summary>
        [HttpDelete("{pricingId}")]
        public async Task<IActionResult> DeletePricing(int pricingId)
        {
            var success = await _pricingService.DeletePricingAsync(pricingId);
            if (!success)
                return NotFound(new { message = "Pricing not found" });
            return Ok(new { message = "Pricing deleted successfully" });
        }

        // ==================== SHIPPING RATE ENDPOINTS ====================

        /// <summary>
        /// Get all shipping rates
        /// </summary>
        [HttpGet("shipping-rates")]
        public async Task<IActionResult> GetAllShippingRates()
        {
            var result = await _pricingService.GetAllShippingRatesAsync();
            return Ok(result);
        }

        /// <summary>
        /// Get shipping rate by ID
        /// </summary>
        [HttpGet("shipping-rates/{shippingRateId}")]
        public async Task<IActionResult> GetShippingRateById(int shippingRateId)
        {
            var result = await _pricingService.GetShippingRateByIdAsync(shippingRateId);
            if (result == null)
                return NotFound(new { message = "Shipping rate not found" });
            return Ok(result);
        }

        /// <summary>
        /// Create new shipping rate
        /// </summary>
        [HttpPost("shipping-rates")]
        public async Task<IActionResult> CreateShippingRate([FromBody] CreateShippingRateRequest request)
        {
            try
            {
                var result = await _pricingService.CreateShippingRateAsync(request);
                return CreatedAtAction(nameof(GetShippingRateById), new { shippingRateId = result.ShippingRateId }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Update existing shipping rate
        /// </summary>
        [HttpPut("shipping-rates/{shippingRateId}")]
        public async Task<IActionResult> UpdateShippingRate(int shippingRateId, [FromBody] UpdateShippingRateRequest request)
        {
            request.ShippingRateId = shippingRateId;
            var result = await _pricingService.UpdateShippingRateAsync(request);
            if (result == null)
                return NotFound(new { message = "Shipping rate not found" });
            return Ok(result);
        }

        /// <summary>
        /// Delete shipping rate
        /// </summary>
        [HttpDelete("shipping-rates/{shippingRateId}")]
        public async Task<IActionResult> DeleteShippingRate(int shippingRateId)
        {
            var success = await _pricingService.DeleteShippingRateAsync(shippingRateId);
            if (!success)
                return NotFound(new { message = "Shipping rate not found" });
            return Ok(new { message = "Shipping rate deleted successfully" });
        }

    }
}
