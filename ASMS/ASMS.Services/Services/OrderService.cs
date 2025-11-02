using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASMS.Repositories.Entities;
using ASMS.Repositories.Infrastructures;
using ASMS.Services.Interfaces;
using ASMS.Services.Model.Orders;
using AutoMapper;

namespace ASMS.Services.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public OrderService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PaginatedOrderResponse> GetWithFilterAsync(int pageNumber, int pageSize, string? customerCode, DateOnly? orderDate, DateOnly? depositDate, DateOnly? returnDate)
        {
            var orders = await _unitOfWork.Orders.GetWithFilterAsync(pageNumber, pageSize, customerCode, orderDate, depositDate, returnDate);
            var totalCount = await _unitOfWork.Orders.GetTotalCountWithFilterAsync(customerCode, orderDate, depositDate, returnDate);

            return new PaginatedOrderResponse
            {
                Data = _mapper.Map<List<OrderResponse>>(orders),
                Page = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
            };
        }

        public async Task<OrderResponse?> GetByCodeAsync(string orderCode)
        {
            var order = await _unitOfWork.Orders.GetByCodeAsync(orderCode);
            return order == null ? null : _mapper.Map<OrderResponse>(order);
        }

        public async Task<OrderResponse> CreateAsync(CreateOrderRequest request)
        {
            var existing = await _unitOfWork.Orders.GetByCodeAsync(request.OrderCode);
            if (existing != null)
                throw new Exception($"Order with code '{request.OrderCode}' already exists.");

            var order = _mapper.Map<Order>(request);
            var created = await _unitOfWork.Orders.AddAsync(order);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<OrderResponse>(created);
        }

        public async Task<OrderResponse> UpdateAsync(string orderCode, UpdateOrderRequest request)
        {
            var existing = await _unitOfWork.Orders.GetByCodeAsync(orderCode);
            if (existing == null)
                throw new Exception($"Order with code '{orderCode}' not found.");

            _mapper.Map(request, existing);
            await _unitOfWork.Orders.UpdateAsync(existing);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<OrderResponse>(existing);
        }
    }
}
