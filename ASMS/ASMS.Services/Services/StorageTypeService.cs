using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASMS.Repositories.Entities;
using ASMS.Repositories.Infrastructures;
using ASMS.Services.Interfaces;
using ASMS.Services.Model.StorageTypes;
using AutoMapper;

namespace ASMS.Services.Services
{
    public class StorageTypeService : IStorageTypeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public StorageTypeService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PaginatedStorageTypeResponse> GetWithFilterAsync(int pageNumber, int pageSize, string? nameContains)
        {
            var storageTypes = await _unitOfWork.StorageTypes.GetWithFilterAsync(pageNumber, pageSize, nameContains);
            var totalCount = await _unitOfWork.StorageTypes.GetTotalCountWithFilterAsync(nameContains);

            return new PaginatedStorageTypeResponse
            {
                Data = _mapper.Map<List<StorageTypeResponse>>(storageTypes),
                Page = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };
        }

        public async Task<StorageTypeResponse?> GetByIdAsync(int id)
        {
            var storageType = await _unitOfWork.StorageTypes.GetByIdAsync(id);
            return storageType == null ? null : _mapper.Map<StorageTypeResponse>(storageType);
        }

        public async Task<StorageTypeResponse> CreateAsync(CreateStorageTypeRequest request)
        {
            var existing = await _unitOfWork.StorageTypes.GetByIdAsync(request.StorageTypeId);
            if (existing != null)
                throw new Exception($"Storage type with id {request.StorageTypeId} already exists.");

            var storageType = _mapper.Map<StorageType>(request);
            var created = await _unitOfWork.StorageTypes.AddAsync(storageType);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<StorageTypeResponse>(created);
        }

        public async Task<StorageTypeResponse> UpdateAsync(int id, UpdateStorageTypeRequest request)
        {
            var existing = await _unitOfWork.StorageTypes.GetByIdAsync(id);
            if (existing == null)
                throw new Exception($"Storage type with id {id} not found.");

            _mapper.Map(request, existing);
            await _unitOfWork.StorageTypes.UpdateAsync(existing);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<StorageTypeResponse>(existing);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var result = await _unitOfWork.StorageTypes.DeleteAsync(id);
            if (result)
                await _unitOfWork.CompleteAsync();
            return result;
        }
    }
}
