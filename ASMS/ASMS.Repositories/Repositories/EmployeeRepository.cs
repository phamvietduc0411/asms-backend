using ASMS.Repositories.Data;
using ASMS.Repositories.Entities;
using ASMS.Repositories.Infrastructures;
using ASMS.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ASMS.Repositories.Repositories
{
    public class EmployeeRepository : GenericRepository<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(VstorageContext context, ILogger logger) : base(context, logger)
        {
        }
        public async Task<Employee?> GetEmployeeByEmailAsync(string email)
        {
            return await _dbSet
                .Include(e => e.EmployeeRole)
                .FirstOrDefaultAsync(e => e.Username == email);
        }
    }
}
