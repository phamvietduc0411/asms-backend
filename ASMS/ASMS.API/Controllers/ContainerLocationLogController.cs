using ASMS.Services.Interfaces;
using ASMS.Services.Model.ContainerLocationLog;
using Microsoft.AspNetCore.Mvc;

namespace ASMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContainerLocationLogController : ControllerBase
    {
        private readonly IContainerLocationLogService _service;

        public ContainerLocationLogController(IContainerLocationLogService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
    [FromQuery] string? containerCode,
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 10)
        {
            var result = await _service.GetWithFilterAsync(containerCode, pageNumber, pageSize);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateContainerLocationLogRequest request)
        {
            var created = await _service.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = created.ContainerLocationLogId }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateContainerLocationLogRequest request)
        {
            var updated = await _service.UpdateAsync(id, request);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _service.DeleteAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }
    }
}
