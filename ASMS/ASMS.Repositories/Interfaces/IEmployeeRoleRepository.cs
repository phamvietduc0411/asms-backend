using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASMS.Repositories.Entities;

namespace ASMS.Repositories.Interfaces
{
    public interface IEmployeeRoleRepository
    {
        Task<EmployeeRole?> GetEntityByIdAsync(int id);
        Task<EmployeeRole> AddAsync(EmployeeRole role);
    }
}
