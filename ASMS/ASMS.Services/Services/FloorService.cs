using ASMS.Repositories.Entities;
using ASMS.Repositories.Infrastructures;
using ASMS.Repositories.Interfaces;
using ASMS.Services.Interfaces;
using ASMS.Services.Model.Floor;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Services
{
    public class FloorService : IFloorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public FloorService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<FloorResponse>> GetAllAsync()
        {
            var entities = await _unitOfWork.Floors.GetAllAsync();
            return _mapper.Map<IEnumerable<FloorResponse>>(entities);
        }

        public async Task<FloorResponse?> GetByCodeAsync(string floorCode)
        {
            var entity = await _unitOfWork.Floors.GetByCodeAsync(floorCode);
            return entity == null ? null : _mapper.Map<FloorResponse>(entity);
        }

        public async Task<FloorResponse> CreateAsync(CreateFloorRequest request)
        {
            var entity = _mapper.Map<Floor>(request);
            await _unitOfWork.Floors.AddAsync(entity);
            await _unitOfWork.CompleteAsync();
            return _mapper.Map<FloorResponse>(entity);
        }

        public async Task<FloorResponse?> UpdateAsync(UpdateFloorRequest request)
        {
            var entity = await _unitOfWork.Floors.GetByCodeAsync(request.FloorCode);
            if (entity == null) return null;

            _mapper.Map(request, entity);
            await _unitOfWork.Floors.UpdateAsync(entity);
            await _unitOfWork.CompleteAsync();
            return _mapper.Map<FloorResponse>(entity);
        }

        public async Task<bool> DeleteAsync(string floorCode)
        {
            var entity = await _unitOfWork.Floors.GetByCodeAsync(floorCode);
            if (entity == null) return false;

            await _unitOfWork.Floors.DeleteAsync(entity);
            await _unitOfWork.CompleteAsync();
            return true;
        }
    }
}
