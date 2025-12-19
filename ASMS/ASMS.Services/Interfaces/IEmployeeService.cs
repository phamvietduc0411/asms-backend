using ASMS.Repositories.Common;
using ASMS.Repositories.Entities;
using ASMS.Services.Model.Employee;

namespace ASMS.Services.Interfaces
{
    public interface IEmployeeService
    {
        Task<Employee?> GetByIdAsync(int id);
        Task<EmployeeDto?> GetByIdDtoAsync(int id);
        Task<Employee> AddEmployeeAsync(CreateEmployeeRequest request);
        Task<EmployeeDto?> UpdateEmployeeAsync(int id, UpdateEmployeeRequest updateInfo);
        Task<PaginatedList<GetEmployeeResponse>> GetWithFilterAsync(string? roleName,string? status ,int pageNumber, int pageSize);
        Task<Employee?> GetDevliveryEmployeeForOder();
        Task<bool> SoftDeleteAsync(int id);
    }
}
