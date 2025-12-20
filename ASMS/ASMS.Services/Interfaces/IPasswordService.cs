using ASMS.Services.Model.Password;
using Microsoft.AspNetCore.Identity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForgotPasswordRequest = ASMS.Services.Model.Password.ForgotPasswordRequest;

namespace ASMS.Services.Interfaces
{
    public interface IPasswordService
    {
        Task<bool> ChangePasswordAsync(ChangePasswordRequest request);
        Task<bool> SendResetLinkAsync(ForgotPasswordRequest request);
        string GenerateRandomPassword(int length);
        Task<bool> SendEmailAsync(string toEmail, string subject, string htmlContent);
        Task<bool> SendEmailAsync(string toEmail, string subject, string htmlContent, byte[]? qrBytes = null);
    }
}
