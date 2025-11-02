using ASMS.Services.Interfaces;
using ASMS.Services.Model.Container;
using Microsoft.AspNetCore.Mvc;

namespace ASMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContainerController : ControllerBase
    {
        private readonly IContainerService _containerService;

        public ContainerController(IContainerService containerService)
        {
            _containerService = containerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _containerService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{code}")]
        public async Task<IActionResult> GetByCode(string code)
        {
            var result = await _containerService.GetByCodeAsync(code);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateContainerRequest request)
        {
            var result = await _containerService.CreateAsync(request);
            return CreatedAtAction(nameof(GetByCode), new { code = result.ContainerCode }, result);
        }

        [HttpPut("{code}")]
        public async Task<IActionResult> Update(string code, [FromBody] UpdateContainerRequest request)
        {
            var result = await _containerService.UpdateAsync(code, request);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        [HttpDelete("{code}")]
        public async Task<IActionResult> Delete(string code)
        {
            var deleted = await _containerService.DeleteAsync(code);
            if (!deleted)
                return NotFound();
            return NoContent();
        }
    }
}
