using ASMS.Services.Interfaces;
using ASMS.Services.Model.Distance;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DistanceController : ControllerBase
    {
        private readonly IDistanceService _distanceService;

        public DistanceController(IDistanceService distanceService)
        {
            _distanceService = distanceService;
        }

        /// <summary>
        /// Calculate distance between two addresses
        /// </summary>
        /// <param name="request">Origin and destination addresses</param>
        /// <returns>Distance calculation result with estimated cost</returns>
        [HttpPost("calculate")]
        public async Task<IActionResult> CalculateDistance([FromBody] DistanceCalculationRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _distanceService.CalculateDistanceAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    status = "ERROR",
                    message = ex.Message
                });
            }
        }

        /// <summary>
        /// Quick test endpoint to check API configuration
        /// </summary>
        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok(new
            {
                status = "OK",
                message = "Distance API is running",
                timestamp = DateTime.UtcNow
            });
        }
    }
}
