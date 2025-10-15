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

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _shelfService.GetAllAsync();
            return Ok(result);
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
