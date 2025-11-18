using ASMS.Services.Interfaces;
using ASMS.Services.Model.Storages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StorageController : ControllerBase
    {
        private readonly IStorageService _storageService;

        public StorageController(IStorageService storageService)
        {
            _storageService = storageService;
        }

        #region Get Storages with Filter
        /// <summary>
        /// Get storages with pagination and optional filters.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetWithFilterAsync(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? buildingCode = null,
            [FromQuery] string? storageTypeName = null,
            [FromQuery] string? productTypeName = null)
        {
            if (pageNumber < 1 || pageSize < 1)
                return BadRequest("Page number and page size must be greater than 0.");

            var result = await _storageService.GetWithFilterAsync(pageNumber, pageSize, buildingCode, storageTypeName, productTypeName);
            return Ok(result);
        }
        #endregion

        #region Get Storage by Code
        /// <summary>
        /// Get a storage by its code.
        /// </summary>
        [HttpGet("{storageCode}")]
        public async Task<IActionResult> GetByCodeAsync(string storageCode)
        {
            var result = await _storageService.GetByCodeAsync(storageCode);
            if (result == null)
                return NotFound($"Storage with code '{storageCode}' not found.");
            return Ok(result);
        }
        #endregion

        #region Create Storage
        /// <summary>
        /// Create a new storage.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateStorageRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _storageService.CreateAsync(request);
                return Created($"/api/Storage/{result.StorageCode}", result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        #endregion

        #region Update Storage
        /// <summary>
        /// Update an existing storage.
        /// </summary>
        [HttpPut("{storageCode}")]
        public async Task<IActionResult> UpdateAsync(string storageCode, [FromBody] UpdateStorageRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _storageService.UpdateAsync(storageCode, request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
        #endregion

        #region Toggle Active/Inactive
        /// <summary>
        /// Toggle storage between active and inactive status.
        /// If changing from active to inactive, will check if storage has related Shelves or Orders.
        /// </summary>
        /// <param name="storageCode">The code of the storage</param>
        /// <returns>Success message with new status</returns>
        [HttpPatch("{storageCode}/toggle-active")]
        public async Task<IActionResult> ToggleActiveAsync(string storageCode)
        {
            try
            {
                var result = await _storageService.ToggleActiveAsync(storageCode);
                if (!result)
                    return NotFound($"Storage with code '{storageCode}' not found.");

                var updated = await _storageService.GetByCodeAsync(storageCode);
                var newStatus = updated?.IsActive == true ? "activated" : "deactivated";

                return Ok(new
                {
                    message = $"Storage '{storageCode}' has been {newStatus} successfully.",
                    isActive = updated?.IsActive
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        #endregion
    }
}
