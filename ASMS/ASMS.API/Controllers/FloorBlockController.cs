using ASMS.Services.Interfaces;
using ASMS.Services.Model.FloorBlocks;
using Microsoft.AspNetCore.Mvc;

namespace ASMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FloorBlockController : ControllerBase
    {
        private readonly IFloorBlockService _floorBlockService;

        public FloorBlockController(IFloorBlockService floorBlockService)
        {
            _floorBlockService = floorBlockService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _floorBlockService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{floorBlockCode}")]
        public async Task<IActionResult> GetByCode(string floorBlockCode)
        {
            var result = await _floorBlockService.GetByCodeAsync(floorBlockCode);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateFloorBlockRequest request)
        {
            var created = await _floorBlockService.CreateAsync(request);
            return CreatedAtAction(nameof(GetByCode), new { floorBlockCode = created.FloorBlockCode }, created);
        }

        [HttpPut("{floorBlockCode}")]
        public async Task<IActionResult> Update(string floorBlockCode, [FromBody] UpdateFloorBlockRequest request)
        {
            var updated = await _floorBlockService.UpdateAsync(floorBlockCode, request);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("{floorBlockCode}")]
        public async Task<IActionResult> Delete(string floorBlockCode)
        {
            var result = await _floorBlockService.DeleteAsync(floorBlockCode);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}
