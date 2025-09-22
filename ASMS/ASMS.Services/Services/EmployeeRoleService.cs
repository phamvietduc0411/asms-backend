using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASMS.Repositories.Entities;
using ASMS.Repositories.Infrastructures;
using ASMS.Services.Interfaces;

namespace ASMS.Services.Services
{
    public class EmployeeRoleService : IEmployeeRoleService
    {
        private readonly IUnitOfWork _unitOfWork;
        public EmployeeRoleService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;   
        }
        public async Task<EmployeeRole?> GetByIdAsync(int id)
        {
            var employeeRole = await _unitOfWork.EmployeeRoles.GetEntityByIdAsync(id);
            if (employeeRole == null)
            {
                return null;
            }

            return employeeRole;
        }
    }
}
