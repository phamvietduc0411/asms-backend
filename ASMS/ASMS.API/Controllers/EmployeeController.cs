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

        [HttpPut("{id}/delete")]
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
    }
}
