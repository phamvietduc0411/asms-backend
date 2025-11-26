using ASMS.Repositories.Entities;
using ASMS.Services.Model.Authentication;

namespace ASMS.Services.Interfaces
{
    public interface IAuthService
    {
        Task<Customer> FindCustomerAsync(string email);
        Task<Employee> FindEmployeeAsync(string email);
        bool Verify(string password, string hashedPassword);
        string GenerateEmployeeToken(Employee employee);
        string GenerateCustomerToken(Customer customer);
        Task<string> GenerateRefreshTokenAsync<T>(T user, bool isEmployee);
        Task<AuthResponse> RefreshTokenAsync(string token);
        Task<bool> LogoutAsync(string refreshToken);


    }
}
