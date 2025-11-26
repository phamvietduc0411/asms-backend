using ASMS.Repositories.Common;
using ASMS.Repositories.Entities;
using ASMS.Repositories.Infrastructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Repositories.Interfaces
{
    public interface ICustomerRepository : IGenericRepository<Customer>
    {
        Task<Customer> GetCustomerByEmailAsync(string email);
        Task<PaginatedList<Customer>> GetAllAsync(int pageNumber, int pageSize);
        Task<Customer> GetLastRecord();
    }
}
