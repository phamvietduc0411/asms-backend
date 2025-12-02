using ASMS.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaintenanceController : ControllerBase
    {
        private readonly IOrderMaintenanceService _maintenanceService;

        public MaintenanceController(IOrderMaintenanceService maintenanceService)
        {
            _maintenanceService = maintenanceService;
        }

        /// <summary>
        /// [MANUAL] Trigger job kiểm tra overdue orders
        /// </summary>
        [HttpPost("check-overdue")]
        public async Task<IActionResult> CheckOverdueOrders()
        {
            try
            {
                await _maintenanceService.CheckAndProcessOverdueOrdersAsync();
                return Ok(new { message = "Overdue orders checked successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// [MANUAL] Trigger job move expired orders (overdue > 3 days)
        /// </summary>
        [HttpPost("move-expired")]
        public async Task<IActionResult> MoveExpiredOrders()
        {
            try
            {
                await _maintenanceService.MoveOldOverdueOrdersToExpiredStorageAsync();
                return Ok(new { message = "Old overdue orders processed successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
