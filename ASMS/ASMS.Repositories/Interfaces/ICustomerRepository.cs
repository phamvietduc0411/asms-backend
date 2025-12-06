using ASMS.Repositories.Common;
using ASMS.Repositories.Entities;
using ASMS.Repositories.Infrastructures;

namespace ASMS.Repositories.Interfaces
{
    public interface ICustomerRepository : IGenericRepository<Customer>
    {
        Task<Customer> GetCustomerByEmailAsync(string email);
        Task<PaginatedList<Customer>> GetAllAsync(int pageNumber, int pageSize);
        Task<Customer> GetLastRecord();
        Task<Customer> GetByCodeAsync(string customerCode);
    }
}
