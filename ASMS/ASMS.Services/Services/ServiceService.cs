using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASMS.Repositories.Infrastructures;
using ASMS.Services.Interfaces;
using ASMS.Services.Model.Services;
using AutoMapper;

namespace ASMS.Services.Services
{
    public class ServiceService : IServiceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ServiceService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PaginatedServiceResponse> GetWithFilterAsync(int pageNumber, int pageSize, string? nameContains, decimal? minPrice, decimal? maxPrice)
        {
            var services = await _unitOfWork.Services.GetWithFilterAsync(pageNumber, pageSize, nameContains, minPrice, maxPrice);
            var totalCount = await _unitOfWork.Services.GetTotalCountWithFilterAsync(nameContains, minPrice, maxPrice);

            var mappedServices = _mapper.Map<List<ServiceResponse>>(services);

            return new PaginatedServiceResponse
            {
                Data = mappedServices,
                Page = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };
        }

        public async Task<ServiceResponse> CreateAsync(CreateServiceRequest request)
        {
            var existing = await _unitOfWork.Services.GetEntityByIdAsync(request.ServiceId);
            if (existing != null)
            {
                throw new Exception($"Service with id {request.ServiceId} already exists.");
            }

            var service = _mapper.Map<ASMS.Repositories.Entities.Service>(request);
            var created = await _unitOfWork.Services.AddAsync(service);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<ServiceResponse>(created);
        }

        public async Task<ServiceResponse> UpdateAsync(int id, UpdateServiceRequest request)
        {
            var existing = await _unitOfWork.Services.GetEntityByIdAsync(id);
            if (existing == null)
            {
                throw new Exception($"Service with id {id} not found.");
            }

            _mapper.Map(request, existing);
            await _unitOfWork.Services.UpdateAsync(existing);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<ServiceResponse>(existing);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var result = await _unitOfWork.Services.DeleteAsync(id);
            if (result)
            {
                await _unitOfWork.CompleteAsync();
            }
            return result;
        }
    }
}
