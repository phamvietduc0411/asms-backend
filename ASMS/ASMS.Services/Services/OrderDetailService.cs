using ASMS.Repositories.Common;
using ASMS.Repositories.Entities;
using ASMS.Repositories.Infrastructures;
using ASMS.Services.Interfaces;
using ASMS.Services.Model.OrderDetail;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Services
{
    public class OrderDetailService : IOrderDetailService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public OrderDetailService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PaginatedList<OrderDetailItemResponse>> GetWithFilterAsync(bool? isPlaced, string? orderCode, string? storageCode, string? status, bool? isDamaged,int pageNumber, int pageSize)
        {
            var result = await _unitOfWork.OrderDetails.GetWithFilterAsync(
        isPlaced, orderCode, storageCode, status, isDamaged, pageNumber, pageSize);

            var mappedItems = _mapper.Map<List<OrderDetailItemResponse>>(result.Items);

            return new PaginatedList<OrderDetailItemResponse>(
                mappedItems,
                result.CurrentPage,
                result.PageSize,
                result.TotalRecords)
            {
                TotalPages = result.TotalPages
            };
        }

        public async Task<OrderDetailItemResponse?> GetByIdAsync(int id)
        {
            var orderDetail = await _unitOfWork.OrderDetails.GetByIdWithDetailsAsync(id);
            return _mapper.Map<OrderDetailItemResponse?>(orderDetail);
        }

        public async Task<IEnumerable<OrderDetailItemResponse>> GetByOrderCodeAsync(string orderCode)
        {
            var orderDetails = await _unitOfWork.OrderDetails.GetByOrderCodeWithDetailsAsync(orderCode);
            return _mapper.Map<IEnumerable<OrderDetailItemResponse>>(orderDetails);
        }

        public async Task<OrderDetailResponse> CreateAsync(CreateOrderDetailRequest request)
        {
            var entity = _mapper.Map<OrderDetail>(request);
            await _unitOfWork.OrderDetails.AddAsync(entity);
            await _unitOfWork.CompleteAsync();
            return _mapper.Map<OrderDetailResponse>(entity);
        }

        public async Task<OrderDetailResponse?> UpdateAsync(int id, UpdateOrderDetailRequest request)
        {
            var existing = await _unitOfWork.OrderDetails.GetByIdNoIncludeAsync(id);
            if (existing == null) return null;

            existing.OrderCode = request.OrderCode ?? existing.OrderCode;
            existing.StorageCode = request.StorageCode;
            existing.ContainerCode = request.ContainerCode; 
            existing.Price = request.Price ?? existing.Price;
            existing.Quantity = request.Quantity ?? existing.Quantity;
            existing.SubTotal = request.SubTotal ?? existing.SubTotal;
            existing.Image = request.Image;
            //existing.ContainerType = request.ContainerType;
            //existing.ContainerQuantity = request.ContainerQuantity;
            await _unitOfWork.OrderDetails.UpdateAsync(existing);
            await _unitOfWork.CompleteAsync();
            return _mapper.Map<OrderDetailResponse>(existing);
        }

        //public async Task<bool> DeleteAsync(int id)
        //{
        //    var existing = await _unitOfWork.OrderDetails.GetByIdAsync(id);
        //    if (existing == null) return false;

        //    _unitOfWork._context.OrderDetails.Remove(existing);
        //    await _unitOfWork.CompleteAsync();
        //    return true;
        //}
        public async Task<OrderDetailResponse?> UpdateStatusAsync(int orderDetailId, string status)
        {
            var orderDetail = await _unitOfWork.OrderDetails.GetByIdNoIncludeAsync(orderDetailId);
            if (orderDetail == null) return null;

            orderDetail.Status = status;
            orderDetail.LastUpdatedDate = GetVietnamToday();

            await _unitOfWork.OrderDetails.UpdateAsync(orderDetail);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<OrderDetailResponse>(orderDetail);
        }

        public async Task<OrderDetailResponse?> UpdateIsDamagedAsync(int orderDetailId, bool isDamaged)
        {
            var orderDetail = await _unitOfWork.OrderDetails.GetByIdNoIncludeAsync(orderDetailId);
            if (orderDetail == null) return null;

            orderDetail.IsDamaged = isDamaged;
            orderDetail.LastUpdatedDate = GetVietnamToday();

            await _unitOfWork.OrderDetails.UpdateAsync(orderDetail);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<OrderDetailResponse>(orderDetail);
        }
        private DateOnly GetVietnamToday()
        {
            try
            {
                var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                var vietnamNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);
                return DateOnly.FromDateTime(vietnamNow);
            }
            catch (TimeZoneNotFoundException)
            {
                try
                {
                    var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Ho_Chi_Minh");
                    var vietnamNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);
                    return DateOnly.FromDateTime(vietnamNow);
                }
                catch
                {
                    var vietnamNow = DateTime.UtcNow.AddHours(7);
                    return DateOnly.FromDateTime(vietnamNow);
                }
            }
        }
    }
}
