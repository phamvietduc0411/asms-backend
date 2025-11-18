using ASMS.Services.Interfaces;
using ASMS.Services.Model.ShelfType;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShelfTypeController : ControllerBase
    {
        private readonly IShelfTypeService _shelfTypeService;

        public ShelfTypeController(IShelfTypeService shelfTypeService)
        {
            _shelfTypeService = shelfTypeService;
        }

        /// <summary>
        /// Retrieves all shelf types with pagination
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllShelfTypes(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                if (pageNumber < 1)
                    return BadRequest(new { success = false, message = "Page number phải >= 1" });

                if (pageSize < 1 || pageSize > 100)
                    return BadRequest(new { success = false, message = "Page size phải từ 1-100" });

                var result = await _shelfTypeService.GetAllAsync(pageNumber, pageSize);

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

        /// <summary>
        /// Retrieves a shelf type by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetShelfTypeById(int id)
        {
            try
            {
                var result = await _shelfTypeService.GetByIdAsync(id);

                if (result == null)
                    return NotFound(new { success = false, message = "Không tìm thấy shelf type" });

                return Ok(new { success = true, data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Creates a new shelf type
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateShelfType([FromBody] CreateShelfTypeRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ", errors = ModelState });

                var result = await _shelfTypeService.CreateAsync(request);

                return CreatedAtAction(
                    nameof(GetShelfTypeById),
                    new { id = result.ShelfTypeId },
                    new { success = true, message = "Tạo shelf type thành công", data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Updates an existing shelf type
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateShelfType(int id, [FromBody] UpdateShelfTypeRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ", errors = ModelState });

                var result = await _shelfTypeService.UpdateAsync(id, request);

                if (result == null)
                    return NotFound(new { success = false, message = "Không tìm thấy shelf type" });

                return Ok(new { success = true, message = "Cập nhật shelf type thành công", data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// Deletes a shelf type
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShelfType(int id)
        {
            try
            {
                var result = await _shelfTypeService.DeleteAsync(id);

                if (!result)
                    return NotFound(new { success = false, message = "Không tìm thấy shelf type" });

                return Ok(new { success = true, message = "Xóa shelf type thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}
