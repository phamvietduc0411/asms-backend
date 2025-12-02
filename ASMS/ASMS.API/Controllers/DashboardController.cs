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

        [HttpGet("Revenue-by-day")]
        public async Task<IActionResult> GetNumberOfOrders([FromQuery] DateOnly? date,[FromQuery] string? status,[FromQuery] bool isWeekly = false)
        {
            try
            {
                var targetDate = date ?? DateOnly.FromDateTime(DateTime.Today);

                var numberOfOrders = await _dashBoardService.GetOrderStatisticsAsync(targetDate, status, isWeekly);

                return Ok(new
                {
                    success = true,
                    date = targetDate.ToString("yyyy-MM-dd"),
                    isWeekly,
                    data = numberOfOrders
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }


    }
}
