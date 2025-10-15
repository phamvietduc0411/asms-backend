using ASMS.Services.Interfaces;
using ASMS.Services.Model.ContainerType;
using Microsoft.AspNetCore.Mvc;

namespace ASMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContainerTypeController : ControllerBase
    {
        private readonly IContainerTypeService _containerTypeService;
        public ContainerTypeController(IContainerTypeService containerTypeService)
        {
            _containerTypeService = containerTypeService;
        }
        #region CRUD Container Type
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var result = await _containerTypeService.GetByIdAsync(id);
            if (result == null)
                return NotFound(new { message = $"Container Type with code {id} not found." });
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddAsync([FromBody] CreateContainerTypeRequest request)
        {
            try
            {
                var result = await _containerTypeService.AddContainerTypeAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    ErrorMessage = ex.Message,
                });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateByIdAsync(int id, [FromBody] UpdateContainerTypeRequest newType)
        {
            if (newType == null)
                return BadRequest(new { message = "Invalid data." });

            var existingContainerType = await _containerTypeService.GetByIdAsync(id);
            if (existingContainerType == null)
                return NotFound(new { message = $"Container Type with id {id} not found." });

            existingContainerType.Volume = newType.Volume;
            existingContainerType.ProductTypeId = newType.ProductTypeId;
            existingContainerType.Name = newType.Name;
            existingContainerType.Status = newType.Status;
            existingContainerType.IsActive = newType.IsActive;
            existingContainerType.Status = newType.Status;

            var updateContainerType = await _containerTypeService.UpdateContainerTypeAsync(existingContainerType);

            if (updateContainerType == null)
                return StatusCode(500, new { message = "Failed to update container type info." });

            return Ok(new
            {
                message = "Update successful.",
                data = updateContainerType
            });
        }

        [HttpDelete("{id}/delete")]
        public async Task<IActionResult> SoftDeleteAsync(int id)
        {
            var existingContainerType = await _containerTypeService.GetByIdAsync(id);
            if (existingContainerType == null)
                return NotFound(new { message = "Not found" });
            existingContainerType.IsActive = false;
            var deleteContainerType = await _containerTypeService.UpdateContainerTypeAsync(existingContainerType);

            if (deleteContainerType == null)
                return StatusCode(500, new { message = "Failed to delete building." });

            return Ok(new { message = "Marked as deleted." });
        }
        #endregion
    }
}

