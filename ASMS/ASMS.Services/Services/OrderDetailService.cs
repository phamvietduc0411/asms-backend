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

        public async Task<PaginatedList<OrderDetailItemResponse>> GetWithFilterAsync(bool? isPlaced, string? orderCode, string? storageCode, int pageNumber, int pageSize)
        {
            var result = await _unitOfWork.OrderDetails.GetWithFilterAsync(
        isPlaced, orderCode, storageCode, pageNumber, pageSize);

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

        public async Task<OrderDetailResponse?> GetByIdAsync(int id)
        {
            var orderDetail = await _unitOfWork.OrderDetails.GetByIdAsync(id);
            return _mapper.Map<OrderDetailResponse?>(orderDetail);
        }

        public async Task<IEnumerable<OrderDetailResponse>> GetByOrderCodeAsync(string orderCode)
        {
            var orderDetails = await _unitOfWork.OrderDetails.GetByOrderCodeAsync(orderCode);
            return _mapper.Map<IEnumerable<OrderDetailResponse>>(orderDetails);
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
            var existing = await _unitOfWork.OrderDetails.GetByIdAsync(id);
            if (existing == null) return null;

            _mapper.Map(request, existing);
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
    }
}
