using ASMS.Services.Interfaces;
using ASMS.Services.Model.Authentication;
using ASMS.Services.Model.Customer;
using Microsoft.AspNetCore.Mvc;

namespace ASMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthenticationController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("customer-login")]
        public async Task<IActionResult> CustomerLogin([FromBody] UserLoginRequest request)
        {
            var response = await LoginAsync(request, isEmployee: false);
            return response.Success ? Ok(response) : Unauthorized(response);
        }

        [HttpPost("employee-login")]
        public async Task<IActionResult> EmployeeLogin([FromBody] UserLoginRequest request)
        {
            var response = await LoginAsync(request, isEmployee: true);
            return response.Success ? Ok(response) : Unauthorized(response);
        }

        private async Task<AuthResponse> LoginAsync(UserLoginRequest request, bool isEmployee)
        {
            try
            {
                if (isEmployee)
                {
                    var employee = await _authService.FindEmployeeAsync(request.Email);
                    if (employee == null)
                        return new AuthResponse { Success = false, ErrorMessage = "Employee not found" };

                    if (!employee.IsActive)
                        return new AuthResponse { Success = false, ErrorMessage = "This account has been deactivated." };

                    if (!_authService.Verify(request.Password, employee.Password))
                        return new AuthResponse { Success = false, ErrorMessage = "Invalid username or password" };

                    // Create Access Token& Refresh Token
                    var accessToken = _authService.GenerateEmployeeToken(employee);
                    var refreshToken = await _authService.GenerateRefreshTokenAsync(employee, isEmployee: true);

                    return new AuthResponse
                    {
                        Success = true,
                        AccessToken = accessToken,
                        RefreshToken = refreshToken
                    };
                }
                else
                {
                    var customer = await _authService.FindCustomerAsync(request.Email);
                    if (customer == null)
                        return new AuthResponse { Success = false, ErrorMessage = "Customer not found" };

                    if (!customer.IsActive)
                        return new AuthResponse { Success = false, ErrorMessage = "This account has been deactivated." };

                    if (!_authService.Verify(request.Password, customer.Password))
                        return new AuthResponse { Success = false, ErrorMessage = "Invalid username or password" };

                    // Create Access Token& Refresh Token
                    var accessToken = _authService.GenerateCustomerToken(customer);
                    var refreshToken = await _authService.GenerateRefreshTokenAsync(customer, isEmployee: false);

                    return new AuthResponse
                    {
                        Success = true,
                        AccessToken = accessToken,
                        RefreshToken = refreshToken
                    };
                }
            }
            catch (Exception ex)
            {
                return new AuthResponse { Success = false, ErrorMessage = "An unexpected error occurred." };
            }
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest token)
        {
            var result = await _authService.RefreshTokenAsync(token.RefreshToken);
            return result.Success ? Ok(result) : Unauthorized(result);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] RefreshTokenRequest refreshToken)
        {
            var success = await _authService.LogoutAsync(refreshToken.RefreshToken);
            if (!success)
                return BadRequest(new { message = "Token Invalid." });

            return Ok(new { message = "Logout Success." });
        }



    }

}
