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

        /// <summary>
        /// Retrieves all containers with pagination
        /// </summary>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10, max: 100)</param>
        /// <returns>List of containers with Type from ContainerType</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllContainers(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {

                var result = await _containerService.GetAllAsync(pageNumber, pageSize);

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

        // PUT: api/container/position
        [HttpPut("position")]
        public async Task<IActionResult> UpdatePosition([FromBody] UpdateContainerPositionRequest request)
        {
            try
            {
                var result = await _containerService.UpdateContainerPositionAsync(request);

                if (!result)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = $"Container {request.ContainerCode} not found"
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Container position updated successfully"
                });
            }
            catch (Exception ex)
            {

                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }
    }
}
