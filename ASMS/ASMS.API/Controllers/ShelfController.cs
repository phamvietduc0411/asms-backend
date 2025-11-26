using ASMS.Services.Interfaces;
using ASMS.Services.Model.Shelves;
using Microsoft.AspNetCore.Mvc;

namespace ASMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShelfController : ControllerBase
    {
        private readonly IShelfService _shelfService;

        public ShelfController(IShelfService shelfService)
        {
            _shelfService = shelfService;
        }

        /// <summary>
        /// Retrieves all shelves with optional storage code filter and pagination
        /// </summary>
        /// <param name="storageCode">Optional filter by storage code</param>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10, max: 100)</param>
        /// <returns>List of shelves</returns>
        [HttpGet]
        public async Task<IActionResult> GetShelves(
            [FromQuery] string? storageCode,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {

                var result = await _shelfService.GetWithFilterAsync(storageCode, pageNumber, pageSize);

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

        [HttpGet("{shelfCode}")]
        public async Task<IActionResult> GetByCode(string shelfCode)
        {
            var result = await _shelfService.GetByCodeAsync(shelfCode);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateShelfRequest request)
        {
            var created = await _shelfService.CreateAsync(request);
            return CreatedAtAction(nameof(GetByCode), new { shelfCode = created.ShelfCode }, created);
        }

        [HttpPut("{shelfCode}")]
        public async Task<IActionResult> Update(string shelfCode, [FromBody] UpdateShelfRequest request)
        {
            var updated = await _shelfService.UpdateAsync(shelfCode, request);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("{shelfCode}")]
        public async Task<IActionResult> Delete(string shelfCode)
        {
            var success = await _shelfService.DeleteAsync(shelfCode);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}
