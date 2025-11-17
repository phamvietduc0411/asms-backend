using ASMS.Repositories.Entities;
using ASMS.Repositories.Infrastructures;
using ASMS.Services.Interfaces;
using ASMS.Services.Model.ContainerLocationLog;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Services
{
    public class ContainerLocationLogService : IContainerLocationLogService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ContainerLocationLogService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ContainerLocationLogResponse>> GetAllAsync()
        {
            var list = await _unitOfWork.ContainerLocationLogs.GetAllAsync();
            return _mapper.Map<IEnumerable<ContainerLocationLogResponse>>(list);
        }

        public async Task<ContainerLocationLogResponse?> GetByIdAsync(int id)
        {
            var entity = await _unitOfWork.ContainerLocationLogs.GetByIdAsync(id);
            return entity == null ? null : _mapper.Map<ContainerLocationLogResponse>(entity);
        }

        public async Task<IEnumerable<ContainerLocationLogResponse>> GetByContainerCodeAsync(string containerCode)
        {
            var list = await _unitOfWork.ContainerLocationLogs.GetByContainerCodeAsync(containerCode);
            return _mapper.Map<IEnumerable<ContainerLocationLogResponse>>(list);
        }

        public async Task<IEnumerable<ContainerLocationLogResponse>> GetByOrderCodeAsync(string orderCode)
        {
            var list = await _unitOfWork.ContainerLocationLogs.GetByOrderCodeAsync(orderCode);
            return _mapper.Map<IEnumerable<ContainerLocationLogResponse>>(list);
        }

        public async Task<ContainerLocationLogResponse> CreateAsync(CreateContainerLocationLogRequest request)
        {
            var entity = _mapper.Map<ContainerLocationLog>(request);
            entity.ContainerCodeNavigation = null;
            entity.UpdatedDate = DateOnly.FromDateTime(DateTime.Now);

            await _unitOfWork.ContainerLocationLogs.AddAsync(entity);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<ContainerLocationLogResponse>(entity);
        }

        public async Task<ContainerLocationLogResponse?> UpdateAsync(int id, UpdateContainerLocationLogRequest request)
        {
            var existing = await _unitOfWork.ContainerLocationLogs.GetByIdAsync(id);
            if (existing == null) return null;

            _mapper.Map(request, existing);

            existing.ContainerCodeNavigation = null;
            existing.UpdatedDate = DateOnly.FromDateTime(DateTime.Now);

            await _unitOfWork.ContainerLocationLogs.UpdateAsync(existing);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<ContainerLocationLogResponse>(existing);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var result = await _unitOfWork.ContainerLocationLogs.DeleteAsync(id);
            if (!result) return false;

            await _unitOfWork.CompleteAsync();
            return true;
        }
    }
}
