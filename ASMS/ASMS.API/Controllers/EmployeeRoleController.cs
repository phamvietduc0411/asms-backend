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

        #endregion

    }
}
