using ASMS.Services.Interfaces;
using ASMS.Services.Model.Password;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using ForgotPasswordRequest = ASMS.Services.Model.Password.ForgotPasswordRequest;

namespace ASMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PasswordController : ControllerBase
    {
        private readonly IPasswordService _passwordService;

        public PasswordController(IPasswordService passwordService)
        {
            _passwordService = passwordService;
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ChangePasswordRequest request)
        {
            try
            {
                var result = await _passwordService.ChangePasswordAsync(request);
                return result
                    ? Ok(new { message = "Reset password successfully" })
                    : BadRequest(new { message = "Failed reset password" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            try
            {
                var result = await _passwordService.SendResetLinkAsync(request);
                return result
                    ? Ok(new { message = "Success!Please check your email to get new password" })
                    : BadRequest(new { message = "Failed reset password" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
