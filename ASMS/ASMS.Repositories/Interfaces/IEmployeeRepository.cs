using ASMS.Repositories.Common;
using ASMS.Repositories.Data;
using ASMS.Repositories.Entities;
using ASMS.Repositories.Infrastructures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ASMS.Repositories.Interfaces
{
    public interface IEmployeeRepository : IGenericRepository<Employee>
    {
        Task<Employee?> GetEmployeeByEmailAsync(string email);
        Task<PaginatedList<Employee>> GetWithFilterAsync(string? roleName, int pageNumber, int pageSize);
        Task<Employee?> GetEntityByIdAsync(int id);
        Task<Employee?> GetByCodeAsync(string employeeCode);

        Task<IEnumerable<Employee>> GetByRoleAsync(string roleName);
        Task<Employee?> GetAvailableEmployeeByRoleAsync(string roleName);
        Task<IEnumerable<Employee>> GetAllAsync();

    }
}
