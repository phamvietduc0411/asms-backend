using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASMS.Repositories.Entities;
using ASMS.Services.Model.TrackingHistories;

namespace ASMS.Services.Interfaces
{
    public interface ITrackingHistoryService
    {
        Task<PaginatedTrackingHistoryResponse> GetWithFilterAsync(int pageNumber, int pageSize, string? orderCode, string? currentAssign, string? nextAssign);
        Task<TrackingHistoryResponse> CreateAsync(CreateTrackingHistoryRequest request);
        Task<TrackingHistoryResponse> UpdateAsync(int id, UpdateTrackingHistoryRequest request);
        Task<bool> DeleteAsync(int id);
        Task<TrackingHistoryResponse> UpdateStatusAsync(UpdateTrackingStatusRequest request);
        Task<OrderTrackingFlowResponse> GetOrderTrackingFlowAsync(string orderCode);
        Task CreateAsync(TrackingHistory newTrackingHistory);
    }
}
