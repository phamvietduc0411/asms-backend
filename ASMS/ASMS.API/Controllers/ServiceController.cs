using ASMS.Services.Interfaces;
using ASMS.Services.Model.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private readonly IServiceService _serviceService;

        public ServiceController(IServiceService serviceService)
        {
            _serviceService = serviceService;
        }

        #region Get Services with Filter
        /// <summary>
        /// Get services with pagination and optional filters.
        /// </summary>
        /// <param name="pageNumber">Current page number (default = 1)</param>
        /// <param name="pageSize">Number of items per page (default = 10)</param>
        /// <param name="nameContains">Filter by Name contains text (optional)</param>
        /// <param name="minPrice">Filter by minimum price (optional, e.g., 20000)</param>
        /// <param name="maxPrice">Filter by maximum price (optional, e.g., 50000)</param>
        /// <returns>A paginated list of services</returns>
        /// <response code="200">Returns a paginated list of services</response>
        /// <response code="400">Invalid request parameters</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        public async Task<IActionResult> GetWithFilterAsync(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? nameContains = null,
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null)
        {
            if (pageNumber < 1 || pageSize < 1)
            {
                return BadRequest("Page number and page size must be greater than 0.");
            }

            if (minPrice.HasValue && maxPrice.HasValue && minPrice > maxPrice)
            {
                return BadRequest("Minimum price cannot be greater than maximum price.");
            }

            var result = await _serviceService.GetWithFilterAsync(pageNumber, pageSize, nameContains, minPrice, maxPrice);
            return Ok(result);
        }
        #endregion

        #region Create Service
        /// <summary>
        /// Create a new service.
        /// </summary>
        /// <param name="request">The service details to create (including ServiceId)</param>
        /// <returns>The created service</returns>
        /// <response code="201">Service created successfully</response>
        /// <response code="400">Invalid request or ID already exists</response>
        /// <response code="500">Internal server error</response>
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateServiceRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _serviceService.CreateAsync(request);
                return Created($"/api/Service/{result.ServiceId}", result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        #endregion

        #region Update Service
        /// <summary>
        /// Update an existing service.
        /// </summary>
        /// <param name="id">The ID of the service to update</param>
        /// <param name="request">The updated service details</param>
        /// <returns>The updated service</returns>
        /// <response code="200">Service updated successfully</response>
        /// <response code="400">Invalid request</response>
        /// <response code="404">Service not found</response>
        /// <response code="500">Internal server error</response>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(int id, [FromBody] UpdateServiceRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _serviceService.UpdateAsync(id, request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
        #endregion

        #region Delete Service
        /// <summary>
        /// Delete a service by its ID.
        /// </summary>
        /// <param name="id">The ID of the service</param>
        /// <returns>No content if successful</returns>
        /// <response code="204">Service deleted successfully</response>
        /// <response code="404">Service not found</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var result = await _serviceService.DeleteAsync(id);
            if (!result)
            {
                return NotFound($"Service with id {id} not found.");
            }
            return Ok(result);
        }
        #endregion
    }
}
