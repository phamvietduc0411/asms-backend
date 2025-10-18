using ASMS.Services.Interfaces;
using ASMS.Services.Model.Customer;
using ASMS.Services.Utilities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ASMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("customer-login")]
        public async Task<IActionResult> CustomerLogin([FromBody] UserLoginRequest request)
        {
            try
            {
                var customer = await _authService.FindCustomerAsync(request.Email);

                if (customer == null)
                    return Unauthorized(new { message = "Customer not found" });

                if (customer != null)
                {
                    if (customer.IsActive)
                    {

                        if (!_authService.Verify(request.Password, customer.Password))
                            return Unauthorized(new { message = "Invalid username or password" });

                        var token = _authService.GenerateCustomerToken(customer.Id, customer.Email);

                        return Ok(new { token });
                    }
                    else
                    {
                        return BadRequest(new
                        {
                            ErrorMessage = "This account has been deactivated. Please contact Admin for further information!"
                        });
                    }
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return NotFound(new
            {
                ErrorMessage = "Wrong UserName or Password"
            });
        }

        [HttpPost("employee-login")]
        public async Task<IActionResult> EmployeeLogin([FromBody] UserLoginRequest request)
        {
            try
            {
                var employee = await _authService.FindEmployeeAsync(request.Email);

                if (employee == null)
                    return Unauthorized(new { message = "Customer not found" });

                if (employee != null)
                {
                    if (employee.IsActive)
                    {
                        var a = _authService.Verify(request.Password, employee.Password);
                        if (!_authService.Verify(request.Password, employee.Password))
                            return Unauthorized(new { message = "Invalid username or password" });

                        var token = _authService.GenerateEmployeeToken(employee.Id, employee.Username!, employee.EmployeeRole!.ToString());

                        return Ok(new { token });
                    }
                    else
                    {
                        return BadRequest(new
                        {
                            ErrorMessage = "This account has been deactivated. Please contact Admin for further information!"
                        });
                    }
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return NotFound(new
            {
                ErrorMessage = "Wrong UserName or Password"
            });
        }
    }

}
