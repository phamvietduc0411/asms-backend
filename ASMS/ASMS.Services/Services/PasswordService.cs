using ASMS.Repositories.Infrastructures;
using ASMS.Repositories.Interfaces;
using ASMS.Services.Interfaces;
using ASMS.Services.Model.Authentication;
using ASMS.Services.Model.Password;
using ASMS.Services.Utilities;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MimeKit;

public class PasswordService : IPasswordService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ProjectMailConfig _mailConfig;

    public PasswordService(IUnitOfWork unitOfWork, IOptions<ProjectMailConfig> mailConfig)
    {
        _unitOfWork = unitOfWork;
        _mailConfig = mailConfig.Value;
    }

    #region Change Password
    public async Task<bool> ChangePasswordAsync(ChangePasswordRequest request)
    {
        var result = request.IsEmployee
            ? await ChangeEmployeePasswordAsync(request)
            : await ChangeCustomerPasswordAsync(request);
        return result;
    }
    private async Task<bool> ChangeEmployeePasswordAsync(ChangePasswordRequest request)
    {
        //get employee
        var employeeInfo = await _unitOfWork.Employee.GetEmployeeByEmailAsync(request.Email);
        if (employeeInfo == null) return false;

        //check pass
        var checkPass = PasswordHasher.VerifyPassword(request.OldPassword, employeeInfo.Password);

        if (checkPass)
        {
            var newPass = PasswordHasher.HashPassword(request.NewPassword);
            employeeInfo.Password = newPass;
            await _unitOfWork.Employee.UpdateAsync(employeeInfo);
            await _unitOfWork.CompleteAsync();
        }

        return checkPass;
    }
    private async Task<bool> ChangeCustomerPasswordAsync(ChangePasswordRequest request)
    {
        //get customer
        var customerInfo = await _unitOfWork.Customer.GetCustomerByEmailAsync(request.Email);
        if (customerInfo == null) return false;

        //check pass
        var checkPass = PasswordHasher.VerifyPassword(request.OldPassword, customerInfo.Password);

        if (checkPass)
        {
            var newPass = PasswordHasher.HashPassword(request.NewPassword);
            customerInfo.Password = newPass;
            await _unitOfWork.Customer.UpdateAsync(customerInfo);
            await _unitOfWork.CompleteAsync();
        }

        return checkPass;
    }
    #endregion

    public async Task<bool> SendResetLinkAsync(ForgotPasswordRequest request)
    {
        var result = request.IsEmployee
            ? await SendResetLinkForEmployeeAsync(request)
            : await SendResetLinkForCustomerAsync(request);
        return result;
    }
    private async Task<bool> SendResetLinkForEmployeeAsync(ForgotPasswordRequest request)
    {
        //get employee
        var employeeInfo = await _unitOfWork.Employee.GetEmployeeByEmailAsync(request.Email);
        if (employeeInfo == null) return false;

        //create new password and Send mail
        var newPass = GenerateRandomPassword(8);
        string emailContent = EmailTemplates.ForgotPassword(request.Email, newPass, _mailConfig.Email);
        var isSent = await SendEmailAsync(request.Email, "Khôi phục mật khẩu ASMS", emailContent);

        var hashPass = PasswordHasher.HashPassword(newPass);
        employeeInfo.Password = hashPass;
        await _unitOfWork.Employee.UpdateAsync(employeeInfo);
        await _unitOfWork.CompleteAsync();

        return isSent;
    }
    private async Task<bool> SendResetLinkForCustomerAsync(ForgotPasswordRequest request)
    {
        //get customer
        var customerInfo = await _unitOfWork.Customer.GetCustomerByEmailAsync(request.Email);
        if (customerInfo == null) return false;

        //create new password and Send mail
        var newPass = GenerateRandomPassword(8);
        string emailContent = EmailTemplates.ForgotPassword(request.Email, newPass, _mailConfig.Email);
        var isSent = await SendEmailAsync(request.Email, "Khôi phục mật khẩu ASMS", emailContent);

        var hashPass = PasswordHasher.HashPassword(newPass);
        customerInfo.Password = hashPass;
        await _unitOfWork.Customer.UpdateAsync(customerInfo);
        await _unitOfWork.CompleteAsync();

        return isSent;
    }


    #region Send Email
    public string GenerateRandomPassword(int length)
    {
        const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
        var random = new Random();
        return new string(Enumerable.Repeat(validChars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
    public async Task<bool> SendEmailAsync(string toEmail, string subject, string htmlContent)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("ASMS", _mailConfig.Email));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = subject;
            message.Body = new TextPart("html") { Text = htmlContent };

            using var client = new SmtpClient();
            await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(_mailConfig.Email, _mailConfig.ApplicationPass);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
 
    #endregion
}
