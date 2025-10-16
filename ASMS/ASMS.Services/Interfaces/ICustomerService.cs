using ASMS.Repositories.Entities;
using ASMS.Services.Model.Customer;

namespace ASMS.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<Customer> GetByIdAsync(int id);
        Task<Customer> AddCustomerAsync(CreateCustomerRequest request);
        Task<Customer> UpdateCustomerAsync(Customer updateInfo);
    }
}
