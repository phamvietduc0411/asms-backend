using ASMS.Services.Interfaces;
using ASMS.Services.Model.Customer;
using ASMS.Services.Utilities;
using Azure.Core;
using Microsoft.AspNetCore.Mvc;

namespace ASMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {

        private readonly ICustomerService _customerService;
        private readonly ILogger<CustomerController> _logger;

        public CustomerController(ICustomerService employeeRoleService, ILogger<CustomerController> logger)
        {
            _customerService = employeeRoleService;
            _logger = logger;
        }

        #region Customer CRUD
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var customer = await _customerService.GetByIdAsync(id);
            if (customer == null)
                return NotFound(new { message = $"Customer with ID {id} not found." });

            return Ok(customer);
        }

        [HttpPost]
        public async Task<IActionResult> AddAsync([FromBody] CreateCustomerRequest request)
        {
            try
            {
                request.Password = PasswordHasher.HashPassword(request.Password);
                var result = await _customerService.AddCustomerAsync(request);
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
        public async Task<IActionResult> UpdateByIdAsync(int id, [FromBody] UpdateCustomerRequest newInfo)
        {
            if (newInfo == null)
                return BadRequest(new { message = "Invalid data." });

            var existingCustomer = await _customerService.GetByIdAsync(id);
            if (existingCustomer == null)
                return NotFound(new { message = $"Role with id {id} not found." });

            existingCustomer.CustomerCode = newInfo.CustomerCode;
            existingCustomer.Phone = newInfo.Phone;
            existingCustomer.Name = newInfo.Name;
            existingCustomer.IsActive = newInfo.IsActive;
            existingCustomer.Address = newInfo.Address;
            existingCustomer.Email = newInfo.Email;
            existingCustomer.Password = PasswordHasher.HashPassword(newInfo.Password);

            var newCustomerInfo = await _customerService.UpdateCustomerAsync(existingCustomer);

            if (newCustomerInfo == null)
                return StatusCode(500, new { message = "Failed to update info customer." });

            return Ok(new
            {
                message = "Update successful.",
                data = newCustomerInfo
            });
        }

        [HttpPut("{id}/delete")]
        public async Task<IActionResult> SoftDeleteAsync(int id)
        {
            var existingCustomer = await _customerService.GetByIdAsync(id);
            if (existingCustomer == null)
                return NotFound(new { message = "Not found" });
            existingCustomer.IsActive = false;
            var deleteCustomer = await _customerService.UpdateCustomerAsync(existingCustomer);

            if (deleteCustomer == null)
                return StatusCode(500, new { message = "Failed to delete customer." });

            return Ok(new { message = "Marked as deleted." });
        }
        #endregion

        /// <summary>
        /// Retrieves all customers with pagination
        /// </summary>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10, max: 100)</param>
        /// <returns>List of customers</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllCustomers(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {

                var result = await _customerService.GetAllAsync(pageNumber, pageSize);

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
