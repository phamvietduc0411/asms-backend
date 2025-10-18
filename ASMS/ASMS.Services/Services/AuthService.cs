using ASMS.Repositories.Entities;
using ASMS.Repositories.Infrastructures;
using ASMS.Services.Interfaces;
using ASMS.Services.Utilities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly TokenService _tokenService;
        public AuthService(IUnitOfWork unitOfWork, IMapper mapper, TokenService tokenService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _tokenService = tokenService;
        }

        public async Task<Customer> FindCustomerAsync(string email)
        {
            var customer = await _unitOfWork.Customer.GetCustomerByEmailAsync(email);
            if (customer == null)
            {
                return null;
            }
            return customer;
        }

        public async Task<Employee> FindEmployeeAsync(string email)
        {
            var employee = await _unitOfWork.Employee.GetEmployeeByEmailAsync(email);
            if (employee == null)
            {
                return null;
            }
            return employee;
        }

        public string GenerateCustomerToken(int customerId, string email)
                    => _tokenService.GenerateCustomerAccessToken(customerId, email);

        public string GenerateEmployeeToken(int employeeId, string email, string role)
                    => _tokenService.GenerateEmployeeAccessToken(employeeId, email, role);


        public bool Verify(string password, string hashedPassword) => PasswordHasher.VerifyPassword(password, hashedPassword);
    }
}
