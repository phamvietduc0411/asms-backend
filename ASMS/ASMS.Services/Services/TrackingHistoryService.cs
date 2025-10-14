using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASMS.Repositories.Entities;
using ASMS.Repositories.Infrastructures;
using ASMS.Services.Interfaces;
using ASMS.Services.Model.TrackingHistories;
using AutoMapper;

namespace ASMS.Services.Services
{
    public class TrackingHistoryService : ITrackingHistoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TrackingHistoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PaginatedTrackingHistoryResponse> GetWithFilterAsync(int pageNumber, int pageSize, string? orderCode)
        {
            var trackingHistories = await _unitOfWork.TrackingHistories.GetWithFilterAsync(pageNumber, pageSize, orderCode);
            var totalCount = await _unitOfWork.TrackingHistories.GetTotalCountWithFilterAsync(orderCode);

            var mappedHistories = _mapper.Map<List<TrackingHistoryResponse>>(trackingHistories);

            return new PaginatedTrackingHistoryResponse
            {
                Data = mappedHistories,
                Page = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };
        }

        public async Task<TrackingHistoryResponse> CreateAsync(CreateTrackingHistoryRequest request)
        {
            var trackingHistory = _mapper.Map<TrackingHistory>(request);
            if (!trackingHistory.CreateAt.HasValue)
            {
                trackingHistory.CreateAt = DateOnly.FromDateTime(DateTime.Now);
            }

            var created = await _unitOfWork.TrackingHistories.AddAsync(trackingHistory);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<TrackingHistoryResponse>(created);
        }

        public async Task<TrackingHistoryResponse> UpdateAsync(int id, UpdateTrackingHistoryRequest request)
        {
            var existing = await _unitOfWork.TrackingHistories.GetEntityByIdAsync(id);
            if (existing == null)
            {
                throw new Exception($"Tracking history with id {id} not found.");
            }

            _mapper.Map(request, existing);
            await _unitOfWork.TrackingHistories.UpdateAsync(existing);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<TrackingHistoryResponse>(existing);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var result = await _unitOfWork.TrackingHistories.DeleteAsync(id);
            if (result)
            {
                await _unitOfWork.CompleteAsync();
            }
            return result;
        }
    }
}
