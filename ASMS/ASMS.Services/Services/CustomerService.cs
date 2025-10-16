using ASMS.Repositories.Entities;
using ASMS.Repositories.Infrastructures;
using ASMS.Services.Interfaces;
using ASMS.Services.Model.Customer;
using AutoMapper;

namespace ASMS.Services.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public CustomerService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<Customer?> GetByIdAsync(int id)
        {
            var customer = await _unitOfWork.Customer.GetEntityByIdAsync(id);
            if (customer == null)
            {
                return null;
            }
            return customer;
        }
        public async Task<Customer> AddCustomerAsync(CreateCustomerRequest request)
        {
            var entity = _mapper.Map<Customer>(request);
            await _unitOfWork.Customer.AddAsync(entity);
            await _unitOfWork.CompleteAsync();
            return entity;
        }

        public async Task<Customer> UpdateCustomerAsync(Customer updateInfo)
        {
            await _unitOfWork.Customer.UpdateAsync(updateInfo);
            await _unitOfWork.CompleteAsync();
            return updateInfo;
        }
    }
}
