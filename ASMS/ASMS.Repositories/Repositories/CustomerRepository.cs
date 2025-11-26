using ASMS.Repositories.Common;
using ASMS.Repositories.Data;
using ASMS.Repositories.Entities;
using ASMS.Repositories.Infrastructures;
using ASMS.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ASMS.Repositories.Repositories
{
    public class CustomerRepository : GenericRepository<Customer>, ICustomerRepository
    {
        public CustomerRepository(VstorageContext context, ILogger logger) : base(context, logger)
        {
        }
        public async Task<Customer?> GetCustomerByEmailAsync(string email) => await _dbSet.FirstOrDefaultAsync(c => c.Email == email);
        public async Task<PaginatedList<Customer>> GetAllAsync(int pageNumber, int pageSize)
        {
            var query = _context.Customers.OrderBy(c => c.CustomerCode);
            return await PaginatedList<Customer>.CreateAsync(query, pageNumber, pageSize);
        }

        public async Task<Customer> GetLastRecord()
        {
            if (_context == null || _context.Customers == null)
                throw new InvalidOperationException("Database context or Building DbSet is not initialized.");

            var lastCustomer = await _context.Customers
                                                        .Where(c => c.CustomerCode != null && c.CustomerCode.StartsWith("CTM"))
                                                        .OrderByDescending(c => c.CustomerCode)
                                                        .FirstOrDefaultAsync();
            return lastCustomer;
        }

    }
}
