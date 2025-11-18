using ASMS.Repositories.Entities;
using ASMS.Repositories.Infrastructures;
using ASMS.Services.Interfaces;
using ASMS.Services.Model.Authentication;
using ASMS.Services.Utilities;
using AutoMapper;

namespace ASMS.Services.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly TokenService _tokenService;
        public AuthService(IUnitOfWork unitOfWork, TokenService tokenService)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
        }

        public async Task<Customer> FindCustomerAsync(string email)
        {
            var customer = await _unitOfWork.Customer.GetCustomerByEmailAsync(email);
            if (customer == null)
            {
                return null;
            }
            return customer;
        }

        public async Task<Employee> FindEmployeeAsync(string email)
        {
            var employee = await _unitOfWork.Employee.GetEmployeeByEmailAsync(email);
            if (employee == null)
            {
                return null;
            }
            return employee;
        }

        public string GenerateCustomerToken(int customerId, string email)
                    => _tokenService.GenerateCustomerAccessToken(customerId, email);

        public string GenerateEmployeeToken(int employeeId, string email, string role)
                    => _tokenService.GenerateEmployeeAccessToken(employeeId, email, role);

        public async Task<string> GenerateRefreshTokenAsync(int userId, bool isEmployee)
                    => await _tokenService.GenerateRefreshTokenAsync(userId, isEmployee);


        public async Task<AuthResponse> RefreshTokenAsync(string token)
             => await _tokenService.RefreshTokenAsync(token);
        public async Task<bool> LogoutAsync(string refreshToken)
        {
            var tokenEntity = await _unitOfWork.RefreshToken.GetByTokenAsync(refreshToken);
            if (tokenEntity == null || tokenEntity.RevokedAt != null)
                return false;

            tokenEntity.RevokedAt = DateTime.UtcNow;
            await _unitOfWork.RefreshToken.UpdateAsync(tokenEntity);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        public bool Verify(string password, string hashedPassword) => PasswordHasher.VerifyPassword(password, hashedPassword);
    }
}
