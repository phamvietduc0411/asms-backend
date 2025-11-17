using ASMS.Services.Interfaces;
using ASMS.Services.Model.ContainerLocationLog;
using Microsoft.AspNetCore.Mvc;

namespace ASMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContainerLocationLogController : ControllerBase
    {
        private readonly IContainerLocationLogService _service;

        public ContainerLocationLogController(IContainerLocationLogService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpGet("container/{code}")]
        public async Task<IActionResult> GetByContainerCode(string code)
        {
            return Ok(await _service.GetByContainerCodeAsync(code));
        }

        [HttpGet("order/{code}")]
        public async Task<IActionResult> GetByOrderCode(string code)
        {
            return Ok(await _service.GetByOrderCodeAsync(code));
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateContainerLocationLogRequest request)
        {
            var result = await _service.CreateAsync(request);
            return Ok(result);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, UpdateContainerLocationLogRequest request)
        {
            var result = await _service.UpdateAsync(id, request);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            return !result ? NotFound() : Ok(new { Message = "Deleted successfully" });
        }
    }
}
