using ASMS.Repositories.Common;
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

        public async Task<PaginatedList<ContainerLocationLogResponse>> GetWithFilterAsync(
    string? containerCode,
    int? orderDetailId,
    int pageNumber,
    int pageSize)
        {
            var logs = await _unitOfWork.ContainerLocationLogs.GetWithFilterAsync(
                containerCode,
                orderDetailId,
                pageNumber,
                pageSize);

            var mappedItems = _mapper.Map<List<ContainerLocationLogResponse>>(logs.Items);

            return new PaginatedList<ContainerLocationLogResponse>(
                mappedItems,
                logs.CurrentPage,
                logs.PageSize,
                logs.TotalRecords);
        }

        public async Task<ContainerLocationLogResponse?> GetByIdAsync(int id)
        {
            var log = await _unitOfWork.ContainerLocationLogs.GetEntityByIdAsync(id);
            return _mapper.Map<ContainerLocationLogResponse?>(log);
        }

        public async Task<ContainerLocationLogResponse> CreateAsync(CreateContainerLocationLogRequest request)
        {
            var entity = _mapper.Map<ContainerLocationLog>(request);
            await _unitOfWork.ContainerLocationLogs.AddAsync(entity);
            await _unitOfWork.CompleteAsync();
            return _mapper.Map<ContainerLocationLogResponse>(entity);
        }

        public async Task<ContainerLocationLogResponse?> UpdateAsync(int id, UpdateContainerLocationLogRequest request)
        {
            var existing = await _unitOfWork.ContainerLocationLogs.GetEntityByIdAsync(id);
            if (existing == null) return null;

            _mapper.Map(request, existing);
            await _unitOfWork.ContainerLocationLogs.UpdateAsync(existing);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<ContainerLocationLogResponse>(existing);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            await _unitOfWork.ContainerLocationLogs.DeleteAsync(id);
            await _unitOfWork.CompleteAsync();
            return true;
        }
    }
}
