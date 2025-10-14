using ASMS.Services.Interfaces;
using ASMS.Services.Model.StorageBlocks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StorageBlockController : ControllerBase
    {
        private readonly IStorageBlockService _storageBlockService;

        public StorageBlockController(IStorageBlockService storageBlockService)
        {
            _storageBlockService = storageBlockService;
        }

        #region Get Storage Blocks with Filter
        /// <summary>
        /// Get storage blocks with pagination and optional filter by storage code.
        /// </summary>
        /// <param name="pageNumber">Current page number (default = 1)</param>
        /// <param name="pageSize">Number of items per page (default = 10)</param>
        /// <param name="storageCode">Filter by Storage Code (optional)</param>
        /// <returns>A paginated list of storage blocks</returns>
        [HttpGet]
        public async Task<IActionResult> GetWithFilterAsync(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? storageCode = null)
        {
            if (pageNumber < 1 || pageSize < 1)
            {
                return BadRequest("Page number and page size must be greater than 0.");
            }

            var result = await _storageBlockService.GetWithFilterAsync(pageNumber, pageSize, storageCode);
            return Ok(result);
        }
        #endregion

        #region Get Storage Block by Code
        /// <summary>
        /// Get a storage block by its code.
        /// </summary>
        /// <param name="storageBlockCode">The code of the storage block</param>
        /// <returns>The storage block details</returns>
        [HttpGet("{storageBlockCode}")]

        public async Task<IActionResult> GetByCodeAsync(string storageBlockCode)
        {
            var result = await _storageBlockService.GetByCodeAsync(storageBlockCode);
            if (result == null)
            {
                return NotFound($"Storage block with code '{storageBlockCode}' not found.");
            }
            return Ok(result);
        }
        #endregion

        #region Create Storage Block
        /// <summary>
        /// Create a new storage block.
        /// </summary>
        /// <param name="request">The storage block details to create</param>
        /// <returns>The created storage block</returns>
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateStorageBlockRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _storageBlockService.CreateAsync(request);
                return Created($"/api/StorageBlock/{result.StorageBlockCode}", result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        #endregion

        #region Update Storage Block
        /// <summary>
        /// Update an existing storage block.
        /// </summary>
        /// <param name="storageBlockCode">The code of the storage block to update</param>
        /// <param name="request">The updated storage block details</param>
        /// <returns>The updated storage block</returns>
        [HttpPut("{storageBlockCode}")]
        public async Task<IActionResult> UpdateAsync(string storageBlockCode, [FromBody] UpdateStorageBlockRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _storageBlockService.UpdateAsync(storageBlockCode, request);
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
        /// Toggle storage block active status (Activate or Deactivate).
        /// </summary>
        /// <param name="storageBlockCode">The code of the storage block</param>
        /// <param name="isActive">Set to true to activate, false to deactivate</param>
        /// <returns>Success status</returns>
        [HttpPatch("{storageBlockCode}/toggle-active")]
        public async Task<IActionResult> ToggleActiveAsync(string storageBlockCode, [FromQuery] bool isActive)
        {
            var result = await _storageBlockService.ToggleActiveAsync(storageBlockCode, isActive);
            if (!result)
            {
                return NotFound($"Storage block with code '{storageBlockCode}' not found.");
            }

            var status = isActive ? "activated" : "deactivated";
            return Ok(new { message = $"Storage block '{storageBlockCode}' has been {status} successfully." });
        }
        #endregion

        #region Soft Delete (Deactivate)
        /// <summary>
        /// Soft delete a storage block by setting IsActive to false.
        /// </summary>
        /// <param name="storageBlockCode">The code of the storage block</param>
        /// <returns>No content if successful</returns>
        [HttpDelete("{storageBlockCode}")]
        public async Task<IActionResult> SoftDeleteAsync(string storageBlockCode)
        {
            var result = await _storageBlockService.SoftDeleteAsync(storageBlockCode);
            if (!result)
            {
                return NotFound($"Storage block with code '{storageBlockCode}' not found.");
            }
            return NoContent();
        }
        #endregion
    }
}
