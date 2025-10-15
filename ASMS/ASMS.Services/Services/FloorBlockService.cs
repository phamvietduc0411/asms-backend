using ASMS.Repositories.Entities;
using ASMS.Repositories.Infrastructures;
using ASMS.Services.Interfaces;
using ASMS.Services.Model.FloorBlocks;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Services
{
    public class FloorBlockService : IFloorBlockService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public FloorBlockService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<FloorBlockResponse>> GetAllAsync()
        {
            var entities = await _unitOfWork.FloorBlocks.GetAllAsync();
            return _mapper.Map<IEnumerable<FloorBlockResponse>>(entities);
        }

        public async Task<FloorBlockResponse?> GetByCodeAsync(string floorBlockCode)
        {
            var entity = await _unitOfWork.FloorBlocks.GetByCodeAsync(floorBlockCode);
            return _mapper.Map<FloorBlockResponse>(entity);
        }

        public async Task<FloorBlockResponse> CreateAsync(CreateFloorBlockRequest request)
        {
            var entity = _mapper.Map<FloorBlock>(request);
            await _unitOfWork.FloorBlocks.AddAsync(entity);
            await _unitOfWork.CompleteAsync();
            return _mapper.Map<FloorBlockResponse>(entity);
        }

        public async Task<FloorBlockResponse?> UpdateAsync(string floorBlockCode, UpdateFloorBlockRequest request)
        {
            var existing = await _unitOfWork.FloorBlocks.GetByCodeAsync(floorBlockCode);
            if (existing == null) return null;

            _mapper.Map(request, existing);
            await _unitOfWork.FloorBlocks.UpdateAsync(existing);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<FloorBlockResponse>(existing);
        }

        public async Task<bool> DeleteAsync(string floorBlockCode)
        {
            var entity = await _unitOfWork.FloorBlocks.GetByCodeAsync(floorBlockCode);
            if (entity == null) return false;

            await _unitOfWork.FloorBlocks.DeleteAsync(floorBlockCode);
            await _unitOfWork.CompleteAsync();
            return true;
        }
    }
}
