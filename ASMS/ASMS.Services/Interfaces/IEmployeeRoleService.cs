using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASMS.Repositories.Entities;
using ASMS.Services.Model.Role;

namespace ASMS.Services.Interfaces
{
    public interface IEmployeeRoleService
    {
        Task<EmployeeRole?> GetByIdAsync(int id);
        Task<EmployeeRole> AddRoleAsync(CreateRoleRequest role);
        Task<EmployeeRole> UpdateRoleAsync(EmployeeRole role);
    }
}
