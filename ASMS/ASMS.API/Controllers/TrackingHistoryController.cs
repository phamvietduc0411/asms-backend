using ASMS.Services.Interfaces;
using ASMS.Services.Model.TrackingHistories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrackingHistoryController : ControllerBase
    {
        private readonly ITrackingHistoryService _trackingHistoryService;

        public TrackingHistoryController(ITrackingHistoryService trackingHistoryService)
        {
            _trackingHistoryService = trackingHistoryService;
        }

        #region Get Tracking Histories with Filter
        /// <summary>
        /// Get tracking histories with pagination and optional filters
        /// </summary>
        /// <param name="pageNumber">Current page number (default = 1)</param>
        /// <param name="pageSize">Number of items per page (default = 10)</param>
        /// <param name="orderCode">Filter by Order Code (optional)</param>
        /// <param name="currentAssign">Filter by Current Assign Employee Code (optional)</param>
        /// <param name="nextAssign">Filter by Next Assign Employee Code (optional)</param>
        /// <returns>A paginated list of tracking histories</returns>
        /// <response code="200">Returns a paginated list of tracking histories</response>
        /// <response code="400">Invalid request parameters</response>
        [HttpGet]
        public async Task<IActionResult> GetWithFilterAsync(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? orderCode = null,
            [FromQuery] string? currentAssign = null,
            [FromQuery] string? nextAssign = null)
        {
            if (pageNumber < 1 || pageSize < 1)
            {
                return BadRequest("Page number and page size must be greater than 0.");
            }

            var result = await _trackingHistoryService.GetWithFilterAsync(
                pageNumber, pageSize, orderCode, currentAssign, nextAssign);

            return Ok(result);
        }
        #endregion

        #region Create Tracking History
        /// <summary>
        /// Create a new tracking history.
        /// </summary>
        /// <param name="request">The tracking history details to create</param>
        /// <returns>The created tracking history</returns>
        /// <response code="201">Tracking history created successfully</response>
        /// <response code="400">Invalid request</response>
        /// <response code="500">Internal server error</response>
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateTrackingHistoryRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _trackingHistoryService.CreateAsync(request);
                return Created($"/api/TrackingHistory/{result.TrackingHistoryId}", result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        #endregion

        [HttpPost("update-status")]
        public async Task<IActionResult> UpdateStatusAsync([FromBody] UpdateTrackingStatusRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _trackingHistoryService.UpdateStatusAsync(request);
                return Created($"/api/TrackingHistory/{result.TrackingHistoryId}", result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpGet("order/{orderCode}")]
        public async Task<IActionResult> GetOrderTrackingFlowAsync(string orderCode)
        {
            try
            {
                var result = await _trackingHistoryService.GetOrderTrackingFlowAsync(orderCode);
                return Ok(new
                {
                    success = true,
                    message = "Tracking flow retrieved successfully",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
        }

        #region Update Tracking History
        /// <summary>
        /// Update an existing tracking history.
        /// </summary>
        /// <param name="id">The ID of the tracking history to update</param>
        /// <param name="request">The updated tracking history details</param>
        /// <returns>The updated tracking history</returns>
        /// <response code="200">Tracking history updated successfully</response>
        /// <response code="400">Invalid request</response>
        /// <response code="404">Tracking history not found</response>
        /// <response code="500">Internal server error</response>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] UpdateTrackingHistoryRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _trackingHistoryService.UpdateAsync(id, request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
        #endregion

        #region Delete Tracking History
        /// <summary>
        /// Delete a tracking history by its ID.
        /// </summary>
        /// <param name="id">The ID of the tracking history</param>
        /// <returns>No content if successful</returns>
        /// <response code="204">Tracking history deleted successfully</response>
        /// <response code="404">Tracking history not found</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var result = await _trackingHistoryService.DeleteAsync(id);
            if (!result)
            {
                return NotFound($"Tracking history with id {id} not found.");
            }
            return Ok(result);
        }
        #endregion
    }
}
