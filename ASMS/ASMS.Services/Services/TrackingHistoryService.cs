using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
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

        public async Task<PaginatedTrackingHistoryResponse> GetWithFilterAsync(int pageNumber, int pageSize, string? orderCode, string? currentAssign, string? nextAssign)
        {
            var trackingHistories = await _unitOfWork.TrackingHistories.GetWithFilterAsync(pageNumber, pageSize, orderCode, currentAssign, nextAssign);
            var totalCount = await _unitOfWork.TrackingHistories.GetTotalCountWithFilterAsync(orderCode, currentAssign, nextAssign);

            var mappedHistories = trackingHistories.Select(th =>
            {
                var response = _mapper.Map<TrackingHistoryResponse>(th);
                response.Image = DeserializeImageUrls(th.Image, th.TrackingHistoryId);
                return response;
            }).ToList();

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

        public async Task<TrackingHistoryResponse> UpdateStatusAsync(UpdateTrackingStatusRequest request)
        {

            var order = await _unitOfWork.Orders.GetByCodeAsync(request.OrderCode);
            if (order == null)
            {
                throw new Exception($"Order with code '{request.OrderCode}' not found.");
            }


            var currentEmployee = await _unitOfWork.Employee.GetByCodeAsync(request.CurrentAssign);
            if (currentEmployee == null)
            {
                throw new Exception($"Employee with code '{request.CurrentAssign}' not found.");
            }

            if (!string.IsNullOrEmpty(request.NextAssign))
            {
                var nextEmployee = await _unitOfWork.Employee.GetByCodeAsync(request.NextAssign);
                if (nextEmployee == null)
                {
                    throw new Exception($"Next assign employee with code '{request.NextAssign}' not found.");
                }
            }


            string newStatus = request.NewStatus;

            // Delivery Staff completing ProgressTask -> Ready
            if (currentEmployee.EmployeeRole?.Name == "Delivery Staff" &&
                request.OldStatus == "ProgressTask")
            {
                newStatus = "Ready";
            }

            // 5. Create new TrackingHistory record
            var trackingHistory = new TrackingHistory
            {
                OrderDetailCode = request.OrderDetailCode,
                OrderCode = request.OrderCode,
                OldStatus = request.OldStatus,
                NewStatus = newStatus,
                ActionType = request.ActionType,
                CreateAt = DateOnly.FromDateTime(DateTime.Now),
                CurrentAssign = request.CurrentAssign,
                NextAssign = request.NextAssign,
                Image = request.Image
            };

            await _unitOfWork.TrackingHistories.AddAsync(trackingHistory);

            // Update Order Status based on NewStatus
            order.Status = newStatus;
            await _unitOfWork.Orders.UpdateAsync(order);


            await _unitOfWork.CompleteAsync();

            return _mapper.Map<TrackingHistoryResponse>(trackingHistory);
        }
        public async Task<OrderTrackingFlowResponse> GetOrderTrackingFlowAsync(string orderCode)
        {
            var order = await _unitOfWork.Orders.GetByCodeAsync(orderCode);
            if (order == null)
            {
                throw new Exception($"Order with code '{orderCode}' not found.");
            }

            // Get all tracking history for this order (ordered by time)
            var trackingHistories = await _unitOfWork.TrackingHistories
                .GetWithFilterAsync(1, 1000, orderCode, null, null); // Get all records

            var mappedHistories = _mapper.Map<List<TrackingHistoryResponse>>(trackingHistories);

            return new OrderTrackingFlowResponse
            {
                OrderCode = orderCode,
                CurrentStatus = order.Status,
                TrackingFlow = mappedHistories,
                TotalSteps = mappedHistories.Count
            };
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

        public async Task CreateAsync(TrackingHistory newTrackingHistory)
        {
            var result = await _unitOfWork.TrackingHistories.AddAsync(newTrackingHistory);
            if (result != null)
            {
                await _unitOfWork.CompleteAsync();
            }
        }
        /// <summary>
        /// Helper method để deserialize Image từ JSON string thành List<string>
        /// </summary>
        private List<string>? DeserializeImageUrls(string? imageJson, int trackingHistoryId)
        {
            if (string.IsNullOrEmpty(imageJson))
                return new List<string>();

            try
            {
                return JsonSerializer.Deserialize<List<string>>(imageJson);
            }
            catch (Exception ex)
            {
                return new List<string>();
            }
        }
    }
}
