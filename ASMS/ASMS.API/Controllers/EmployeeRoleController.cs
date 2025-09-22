using ASMS.Services.Interfaces;
using Microsoft.AspNetCore.Http;
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
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var employeeRole = await _employeeRoleService.GetByIdAsync(id);
            if (employeeRole == null)
            {
                return NotFound(new { message = $"Role with ID {id} not found." });
            }
            return Ok(employeeRole);
        }
    }
}
