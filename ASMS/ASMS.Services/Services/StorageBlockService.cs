using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASMS.Repositories.Entities;
using ASMS.Repositories.Infrastructures;
using ASMS.Services.Interfaces;
using ASMS.Services.Model.StorageBlocks;
using AutoMapper;

namespace ASMS.Services.Services
{
    public class StorageBlockService : IStorageBlockService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public StorageBlockService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PaginatedStorageBlockResponse> GetWithFilterAsync(int pageNumber, int pageSize, string? storageCode)
        {
            var storageBlocks = await _unitOfWork.StorageBlocks.GetWithFilterAsync(pageNumber, pageSize, storageCode);
            var totalCount = await _unitOfWork.StorageBlocks.GetTotalCountWithFilterAsync(storageCode);

            var mappedBlocks = _mapper.Map<List<StorageBlockResponse>>(storageBlocks);

            return new PaginatedStorageBlockResponse
            {
                Data = mappedBlocks,
                Page = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };
        }

        public async Task<StorageBlockResponse?> GetByCodeAsync(string storageBlockCode)
        {
            var storageBlock = await _unitOfWork.StorageBlocks.GetByCodeAsync(storageBlockCode);
            return storageBlock == null ? null : _mapper.Map<StorageBlockResponse>(storageBlock);
        }

        public async Task<StorageBlockResponse> CreateAsync(CreateStorageBlockRequest request)
        {
            var existing = await _unitOfWork.StorageBlocks.GetByCodeAsync(request.StorageBlockCode);
            if (existing != null)
            {
                throw new Exception($"Storage block with code '{request.StorageBlockCode}' already exists.");
            }

            var storageBlock = _mapper.Map<StorageBlock>(request);

            storageBlock.IsActive = true;

            var created = await _unitOfWork.StorageBlocks.AddAsync(storageBlock);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<StorageBlockResponse>(created);
        }

        public async Task<StorageBlockResponse> UpdateAsync(string storageBlockCode, UpdateStorageBlockRequest request)
        {
            var existing = await _unitOfWork.StorageBlocks.GetByCodeAsync(storageBlockCode);
            if (existing == null)
            {
                throw new Exception($"Storage block with code '{storageBlockCode}' not found.");
            }

            _mapper.Map(request, existing);
            await _unitOfWork.StorageBlocks.UpdateAsync(existing);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<StorageBlockResponse>(existing);
        }

        public async Task<bool> ToggleActiveAsync(string storageBlockCode, bool isActive)
        {
            var existing = await _unitOfWork.StorageBlocks.GetByCodeAsync(storageBlockCode);
            if (existing == null)
            {
                return false;
            }

            existing.IsActive = isActive;
            await _unitOfWork.StorageBlocks.UpdateAsync(existing);
            await _unitOfWork.CompleteAsync();

            return true;
        }

        public async Task<bool> SoftDeleteAsync(string storageBlockCode)
        {
            var result = await _unitOfWork.StorageBlocks.SoftDeleteAsync(storageBlockCode);
            if (result)
            {
                await _unitOfWork.CompleteAsync();
            }
            return result;
        }

    }
}
