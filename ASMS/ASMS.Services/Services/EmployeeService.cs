using ASMS.Repositories.Common;
using ASMS.Repositories.Entities;
using ASMS.Repositories.Infrastructures;
using ASMS.Services.Interfaces;
using ASMS.Services.Model.Customer;
using ASMS.Services.Model.Employee;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public EmployeeService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<Employee?> GetByIdAsync(int id)
        {
            var employee = await _unitOfWork.Employee.GetEntityByIdAsync(id);
            if (employee == null)
            {
                return null;
            }
            return employee;
        }
        public async Task<Employee> AddEmployeeAsync(CreateEmployeeRequest request)
        {
            var entity = _mapper.Map<Employee>(request);
            await _unitOfWork.Employee.AddAsync(entity);
            await _unitOfWork.CompleteAsync();
            return entity;
        }

        public async Task<Employee> UpdateEmployeeAsync(Employee updateInfo)
        {
            await _unitOfWork.Employee.UpdateAsync(updateInfo);
            await _unitOfWork.CompleteAsync();
            return updateInfo;
        }
        public async Task<PaginatedList<GetEmployeeResponse>> GetWithFilterAsync(string? roleName,string? status ,int pageNumber, int pageSize)
        {
            var result = await _unitOfWork.Employee.GetWithFilterAsync(roleName,status ,pageNumber, pageSize);

            var mappedItems = _mapper.Map<List<GetEmployeeResponse>>(result.Items);

            return new PaginatedList<GetEmployeeResponse>(
                mappedItems,
                result.CurrentPage,
                result.PageSize,
                result.TotalRecords)
            {
                TotalPages = result.TotalPages
            };
        }

        public async Task<Employee?> GetDevliveryEmployeeForOder()
        {
            var employee = await _unitOfWork.Employee.GetAvailableDeliveryForOrder();
            return employee;
        }
    }
}
