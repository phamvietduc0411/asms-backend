using ASMS.Services.Interfaces;
using ASMS.Services.Model.StorageTypes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StorageTypeController : ControllerBase
    {
        private readonly IStorageTypeService _storageTypeService;

        public StorageTypeController(IStorageTypeService storageTypeService)
        {
            _storageTypeService = storageTypeService;
        }

        #region Get Storage Types with Filter
        /// <summary>
        /// Get storage types with pagination and optional name filter.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetWithFilterAsync(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? nameContains = null)
        {
            if (pageNumber < 1 || pageSize < 1)
                return BadRequest("Page number and page size must be greater than 0.");

            var result = await _storageTypeService.GetWithFilterAsync(pageNumber, pageSize, nameContains);
            return Ok(result);
        }
        #endregion

        #region Get Storage Type by Id
        /// <summary>
        /// Get a storage type by its ID.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var result = await _storageTypeService.GetByIdAsync(id);
            if (result == null)
                return NotFound($"Storage type with id {id} not found.");
            return Ok(result);
        }
        #endregion

        #region Create Storage Type
        /// <summary>
        /// Create a new storage type.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateStorageTypeRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _storageTypeService.CreateAsync(request);
                return Created($"/api/StorageType/{result.StorageTypeId}", result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        #endregion

        #region Update Storage Type
        /// <summary>
        /// Update an existing storage type.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] UpdateStorageTypeRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _storageTypeService.UpdateAsync(id, request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
        #endregion

        #region Delete Storage Type
        /// <summary>
        /// Delete a storage type by its ID.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var result = await _storageTypeService.DeleteAsync(id);
            if (!result)
                return NotFound($"Storage type with id {id} not found.");
            return NoContent();
        }
        #endregion
    }
}
