using ASMS.Repositories.Common;
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

        public async Task<Employee?> GetByCodeAsync(string employeeCode)
        {
            return await _dbSet
                .Include(e => e.EmployeeRole)
                .FirstOrDefaultAsync(e => e.EmployeeCode == employeeCode);
        }


        public async Task<PaginatedList<Employee>> GetWithFilterAsync(string? roleName, int pageNumber, int pageSize)
        {
            var query = _context.Employees
                .Include(e => e.EmployeeRole)
                .AsQueryable();

            if (!string.IsNullOrEmpty(roleName))
            {
                query = query.Where(e => e.EmployeeRole != null && e.EmployeeRole.Name == roleName);
            }

            query = query.OrderBy(e => e.EmployeeCode);

            return await PaginatedList<Employee>.CreateAsync(query, pageNumber, pageSize);
        }
        public virtual async Task<Employee?> GetEntityByIdAsync(int id)
        {
            return await _dbSet
                .Include(e => e.EmployeeRole) 
                .FirstOrDefaultAsync(e => e.Id == id);
        }
    }
}
