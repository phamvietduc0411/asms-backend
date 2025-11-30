using ASMS.Services.Interfaces;
using ASMS.Services.Model.Employee;
using ASMS.Services.Utilities;
using Azure.Core;
using Microsoft.AspNetCore.Mvc;

namespace ASMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly ILogger<EmployeeController> _logger;

        public EmployeeController(IEmployeeService employeeRoleService, ILogger<EmployeeController> logger)
        {
            _employeeService = employeeRoleService;
            _logger = logger;
        }

        #region Customer CRUD
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var employee = await _employeeService.GetByIdAsync(id);
            if (employee == null)
                return NotFound(new { message = $"Employee with ID {id} not found." });

            return Ok(employee);
        }

        [HttpPost]
        public async Task<IActionResult> AddAsync([FromBody] CreateEmployeeRequest request)
        {
            try
            {
                request.Password = PasswordHasher.HashPassword(request.Password);
                var result = await _employeeService.AddEmployeeAsync(request);
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
        public async Task<IActionResult> UpdateByIdAsync(int id, [FromBody] UpdateEmployeeRequest newInfo)
        {
            if (newInfo == null)
                return BadRequest(new { message = "Invalid data." });

            var existingEmployee = await _employeeService.GetByIdAsync(id);
            if (existingEmployee == null)
                return NotFound(new { message = $"Employee with id {id} not found." });

            existingEmployee.EmployeeCode = newInfo.EmployeeCode;
            existingEmployee.EmployeeRoleId = newInfo.EmployeeRoleId;
            existingEmployee.Name = newInfo.Name;
            existingEmployee.BuildingId = newInfo.BuildingId;
            existingEmployee.Phone = newInfo.Phone;
            existingEmployee.Address = newInfo.Address;
            existingEmployee.Username = newInfo.Username;
            existingEmployee.Password = PasswordHasher.HashPassword(newInfo.Password);
            existingEmployee.Status = newInfo.Status;
            existingEmployee.IsActive = newInfo.IsActive;

            var newEmployeeInfo = await _employeeService.UpdateEmployeeAsync(existingEmployee);

            if (newEmployeeInfo == null)
                return StatusCode(500, new { message = "Failed to update info employee." });

            return Ok(new
            {
                message = "Update successful.",
                data = newEmployeeInfo
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDeleteAsync(int id)
        {
            var existingEmployee = await _employeeService.GetByIdAsync(id);
            if (existingEmployee == null)
                return NotFound(new { message = "Not found" });
            existingEmployee.IsActive = false;
            var deleteEmployee = await _employeeService.UpdateEmployeeAsync(existingEmployee);

            if (deleteEmployee == null)
                return StatusCode(500, new { message = "Failed to delete employee." });

            return Ok(new { message = "Marked as deleted." });
        }
        #endregion
        /// <summary>
        /// Retrieves all employees with optional role filter and pagination
        /// </summary>
        /// <param name="roleName">Optional role name to filter employees with 3 role Manager, Delivery Staff, Warehouse Staff</param>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10, max: 100)</param>
        /// <returns>List of employees with RoleName from EmployeeRole</returns>
        [HttpGet]
        public async Task<IActionResult> GetEmployees(
            [FromQuery] string? roleName,
            [FromQuery] string? status,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {

                var result = await _employeeService.GetWithFilterAsync(roleName,status, pageNumber, pageSize);

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
