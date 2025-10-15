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

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _floorService.GetAllAsync();
            return Ok(result);
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
