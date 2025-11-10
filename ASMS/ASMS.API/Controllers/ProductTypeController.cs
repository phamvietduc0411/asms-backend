using ASMS.Services.Interfaces;
using ASMS.Services.Model;
using ASMS.Services.Model.ProductType;
using ASMS.Services.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductTypeController : ControllerBase
    {
        private readonly IProductTypeService _productTypeService;
        private readonly ILogger<ProductTypeController> _logger;

        public ProductTypeController(IProductTypeService productTypeService, ILogger<ProductTypeController> logger)
        {
            _productTypeService = productTypeService;
            _logger = logger;
        }

        #region ProductType CRUD
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var productType = await _productTypeService.GetByIdAsync(id);
            if (productType == null)
                return NotFound(new { message = $"Product Type with ID {id} not found." });

            return Ok(productType);
        }

        [HttpPost]
        public async Task<IActionResult> AddAsync([FromBody] CreateTypeRequest createTypeRequest)
        {
            try
            {
                var result = await _productTypeService.AddProductTypeAsync(createTypeRequest);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    ErrorMessage = ex.Message,
                    InnerException = ex.InnerException?.Message
                });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateByIdAsync(int id, [FromBody] UpdateTypeRequest newType)
        {
            if (newType == null)
                return BadRequest(new { message = "Invalid data." });

            var existingType = await _productTypeService.GetByIdAsync(id);
            if (existingType == null)
                return NotFound(new { message = $"Role with id {id} not found." });
            existingType.Name = newType.Name;
            existingType.Status = newType.Status;
            existingType.IsActive= newType.IsActive;

            var updatedType = await _productTypeService.UpdateProductTypeAsync(existingType);

            if (updatedType == null)
                return StatusCode(500, new { message = "Failed to update product type." });

            return Ok(new
            {
                message = "Update successful.",
                data = updatedType
            });
        }

        [HttpDelete("{id}/delete")]
        public async Task<IActionResult> SoftDeleteAsync(int id)
        {
            var existingType = await _productTypeService.GetByIdAsync(id);
            if (existingType == null)
                return NotFound(new { message = "Not found" });
            existingType.IsActive = false;
            var deleteRole = await _productTypeService.UpdateProductTypeAsync(existingType);

            if (deleteRole == null)
                return StatusCode(500, new { message = "Failed to delete employee role." });

            return Ok(new { message = "Marked as deleted." });
        }

        #endregion
        /// <summary>
        /// Retrieves all product types with optional isActive filter and pagination
        /// </summary>
        /// <param name="isActive">Optional filter by active status</param>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10, max: 100)</param>
        /// <returns>List of product types</returns>
        [HttpGet]
        public async Task<IActionResult> GetProductTypes(
            [FromQuery] bool? isActive,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {

                var result = await _productTypeService.GetWithFilterAsync(isActive, pageNumber, pageSize);

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
    }
}
