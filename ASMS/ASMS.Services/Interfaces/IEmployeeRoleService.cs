using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASMS.Repositories.Entities;

namespace ASMS.Services.Interfaces
{
    public interface IEmployeeRoleService
    {
        Task<EmployeeRole?> GetByIdAsync(int id);
    }
}
