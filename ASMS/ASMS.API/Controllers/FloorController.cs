using ASMS.Repositories.Entities;
using ASMS.Services.Interfaces;
using ASMS.Services.Model.Floor;
using Microsoft.AspNetCore.Mvc;

namespace ASMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FloorController : ControllerBase
    {
        private readonly IFloorService _floorService;

        public FloorController(IFloorService floorService)
        {
            _floorService = floorService;
        }

        /// <summary>
        /// Retrieves all floors with optional shelf filter and pagination
        /// </summary>
        /// <param name="shelfCode">Optional filter by shelf code</param>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10, max: 100)</param>
        /// <returns>List of floors</returns>
        [HttpGet]
        public async Task<IActionResult> GetFloors(
            [FromQuery] string? shelfCode,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                if (pageNumber < 1)
                    return BadRequest(new { success = false, message = "Page number phải >= 1" });

                if (pageSize < 1 || pageSize > 100)
                    return BadRequest(new { success = false, message = "Page size phải từ 1-100" });

                var result = await _floorService.GetWithFilterAsync(shelfCode, pageNumber, pageSize);

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

        [HttpGet("{floorCode}")]
        public async Task<IActionResult> GetByCode(string floorCode)
        {
            var result = await _floorService.GetByCodeAsync(floorCode);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateFloorRequest request)
        {
            var result = await _floorService.CreateAsync(request);
            return Ok(result);
        }

        [HttpPut("{floorCode}")]
        public async Task<IActionResult> Update(string floorCode, [FromBody] UpdateFloorRequest request)
        {
            request.FloorCode = floorCode;
            var result = await _floorService.UpdateAsync(request);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        [HttpDelete("{floorCode}")]
        public async Task<IActionResult> Delete(string floorCode)
        {
            var success = await _floorService.DeleteAsync(floorCode);
            if (!success)
                return NotFound();
            return Ok();
        }
    }
}
