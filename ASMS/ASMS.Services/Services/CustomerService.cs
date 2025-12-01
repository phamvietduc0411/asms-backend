using ASMS.Repositories.Common;
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
        public async Task<PaginatedList<GetCustomerResponse>> GetAllAsync(int pageNumber, int pageSize)
        {
            var result = await _unitOfWork.Customer.GetAllAsync(pageNumber, pageSize);

            var mappedItems = _mapper.Map<List<GetCustomerResponse>>(result.Items);

            return new PaginatedList<GetCustomerResponse>(
                mappedItems,
                result.CurrentPage,
                result.PageSize,
                result.TotalRecords)
            {
                TotalPages = result.TotalPages
            };
        }

        public async Task<string> GetLastRecord()
        {
            var latRecord = await _unitOfWork.Customer.GetLastRecord();

            int nextNumber = 1;

            if (latRecord != null && !string.IsNullOrWhiteSpace(latRecord.CustomerCode))
            {
                string code = latRecord.CustomerCode.Trim();

                if (code.StartsWith("CTM", StringComparison.OrdinalIgnoreCase))
                {
                    string numberPart = code.Substring(3);
                    if (int.TryParse(numberPart, out int currentNumber))
                    {
                        nextNumber = currentNumber + 1;
                    }
                }
            }
            string newCode = $"CTM{nextNumber:D3}";

            return newCode;
        }
        public async Task<Customer> GetByCodeAsync(string customerCode)
        {
            var customer = await _unitOfWork.Customer.GetByCodeAsync(customerCode);

            if (customer == null)
                return null;

            return customer;    
        }
    }
}
