using ASMS.Services.Interfaces;
using ASMS.Services.Model;
using Microsoft.AspNetCore.Mvc;

namespace ASMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeRoleController : ControllerBase
    {
        private readonly IEmployeeRoleService _employeeRoleService;
        private readonly ILogger<EmployeeRoleController> _logger;

        public EmployeeRoleController(IEmployeeRoleService employeeRoleService, ILogger<EmployeeRoleController> logger)
        {
            _employeeRoleService = employeeRoleService;
            _logger = logger;
        }

        #region Employee CRUD
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var employeeRole = await _employeeRoleService.GetByIdAsync(id);
            if (employeeRole == null)
                return NotFound(new { message = $"Role with ID {id} not found." });

            return Ok(employeeRole);
        }

        [HttpPost]
        public async Task<IActionResult> AddAsync([FromBody] CreateRoleRequest createRoleRequest)
        {
            try
            {
                var result = await _employeeRoleService.AddRoleAsync(createRoleRequest);
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
        public async Task<IActionResult> UpdateByIdAsync(int id, [FromBody] UpdateRoleRequest newRole)
        {
            if (newRole == null)
                return BadRequest(new { message = "Invalid data." });

            var existingRole = await _employeeRoleService.GetByIdAsync(id);
            if (existingRole == null)
                return NotFound(new { message = $"Role with id {id} not found." });
            existingRole.Name = newRole.RoleName;
            existingRole.IsActive = newRole.IsActive;

            var updatedRole = await _employeeRoleService.UpdateRoleAsync(existingRole);

            if (updatedRole == null)
                return StatusCode(500, new { message = "Failed to update employee role." });

            return Ok(new
            {
                message = "Update successful.",
                data = updatedRole
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDeleteAsync(int id)
        {
            var existingRole = await _employeeRoleService.GetByIdAsync(id);
            if (existingRole == null)
                return NotFound(new { message = "Not found" });
            existingRole.IsActive = false;
            var deleteRole = await _employeeRoleService.UpdateRoleAsync(existingRole);

            if (deleteRole == null)
                return StatusCode(500, new { message = "Failed to delete employee role." });

            return Ok(new { message = "Marked as deleted." });
        }


        #endregion

        /// <summary>
        /// Retrieves all employee roles with pagination
        /// </summary>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10, max: 100)</param>
        /// <returns>List of employee roles</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllEmployeeRoles(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {

                var result = await _employeeRoleService.GetAllAsync(pageNumber, pageSize);

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
