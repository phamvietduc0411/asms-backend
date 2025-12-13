using ASMS.Services.Interfaces;
using ASMS.Services.Model.Customer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;

namespace ASMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashBoardService;
        public DashboardController(IDashboardService dashBoardService)
        {
            _dashBoardService = dashBoardService;
        }

        [HttpGet("Get-Number-of-Orders")]
        public async Task<IActionResult> GetNumberOfOrders(
    [FromQuery] DateOnly? date,
    [FromQuery] string? status,
    [FromQuery] string type = "month")
        {
            try
            {
                type = type.ToLower();

                // Validation
                if (type != "day" && type != "week" && type != "month" && type != "year")
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Type must be 'day', 'week', 'month', or 'year'"
                    });
                }

                var targetDate = date ?? DateOnly.FromDateTime(DateTime.Today);
                var result = await _dashBoardService.GetOrderStatisticsAsync(targetDate, status, type);

                return Ok(new
                {
                    success = true,
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("Revenue")]
        public async Task<IActionResult> GetRevenueAsync(
            [FromQuery] DateOnly? date,
            [FromQuery] string type = "month") 
        {
            try
            {
                type = type.ToLower();
                if (type != "day" && type != "week" && type != "month" && type != "year")
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Type must be 'day', 'week', 'month', or 'year'"
                    });
                }
                var targetDate = date ?? DateOnly.FromDateTime(DateTime.Today);

                var revenue = await _dashBoardService.GetRevenueAsync(targetDate, type);

                return Ok(new
                {
                    success = true,
                    date = targetDate.ToString("yyyy-MM-dd"),
                    type,
                    revenue
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
        /// <summary>
        /// Get warehouse usage grouped by Building
        /// </summary>
        [HttpGet("building-usage-summary")]
        public async Task<IActionResult> GetBuildingUsageSummaryAsync()
        {
            try
            {
                var usage = await _dashBoardService.GetBuildingUsageSummaryAsync();
                return Ok(new
                {
                    success = true,
                    data = usage
                });
            }
            catch (Exception ex)
            {

                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("warehouse-usage-percent")]
        public async Task<IActionResult> GetWarehouseUsagePercentAsync()
        {
            try
            {
                var usage = await _dashBoardService.GetWarehouseUsagePercentAsync();
                return Ok(new
                {
                    success = true,
                    warehouseUsage = usage
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }




    }
}
