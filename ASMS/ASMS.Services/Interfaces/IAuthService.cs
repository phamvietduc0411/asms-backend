using ASMS.Repositories.Entities;

namespace ASMS.Services.Interfaces
{
    public interface IAuthService
    {
        Task<Customer> FindCustomerAsync(string email);
        Task<Employee> FindEmployeeAsync(string email);
        bool Verify(string password, string hashedPassword);
        string GenerateEmployeeToken(int employeeId, string email, string role);
        string GenerateCustomerToken(int customerId, string email);
    }
}
