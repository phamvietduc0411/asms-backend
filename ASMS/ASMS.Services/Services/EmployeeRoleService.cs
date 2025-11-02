using ASMS.Repositories.Entities;
using ASMS.Repositories.Infrastructures;
using ASMS.Services.Interfaces;
using ASMS.Services.Model;
using AutoMapper;

namespace ASMS.Services.Services
{
    public class EmployeeRoleService : IEmployeeRoleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public EmployeeRoleService(IUnitOfWork unitOfWork,IMapper mapper)
        {
            _unitOfWork = unitOfWork;   
            _mapper = mapper;
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

        public async Task<EmployeeRole> AddRoleAsync(CreateRoleRequest role)
        {
            var entity = _mapper.Map<EmployeeRole>(role);
            await _unitOfWork.EmployeeRoles.AddAsync(entity);
            await _unitOfWork.CompleteAsync();
            return entity;
        }

        public async Task<EmployeeRole> UpdateRoleAsync(EmployeeRole role)
        {
            await _unitOfWork.EmployeeRoles.UpdateAsync(role);
            await _unitOfWork.CompleteAsync();
            return role;
        }
    }
}
