using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASMS.Repositories.Entities;
using ASMS.Repositories.Infrastructures;
using ASMS.Services.Interfaces;
using ASMS.Services.Model.CLP;
using ASMS.Services.Model.OrderDetail;
using ASMS.Services.Model.Orders;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace ASMS.Services.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<OrderService> _logger;
        private readonly ICLPService _clpService;

        public OrderService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<OrderService> logger, ICLPService clpService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _clpService = clpService;
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

        public async Task<CreateOrderResponse> CreateAsync(CreateOrderRequest request)
        {
            _logger.LogInformation("Creating new order for customer {Code}", request.CustomerCode);

            var orderDate = DateOnly.FromDateTime(DateTime.Now);
            var orderCode = await GenerateOrderCodeAsync(orderDate);

            var order = new Order
            {
                OrderCode = orderCode,
                CustomerCode = request.CustomerCode,
                OrderDate = orderDate,
                DepositDate = request.DepositDate,
                ReturnDate = request.ReturnDate,
                Status = request.Status ?? "Pending",
                PaymentStatus = request.PaymentStatus ?? "Unpaid",
                TotalPrice = 0,
                UnpaidAmount = 0
            };

            await _unitOfWork.Orders.AddAsync(order);
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation("Order {OrderCode} created successfully", orderCode);

            return new CreateOrderResponse
            {
                OrderCode = orderCode,
                CustomerCode = request.CustomerCode,
                OrderDate = order.OrderDate,
                DepositDate = order.DepositDate,
                ReturnDate = order.ReturnDate,
                Status = order.Status,
                PaymentStatus = order.PaymentStatus,
                TotalPrice = order.TotalPrice,
                UnpaidAmount = order.UnpaidAmount
            };

        }
        //Tạo Order detail và assign container
        public async Task<CreateOrderDetailResponse> CreateOrderDetailAsync(CreateOrderDetailRequest request)
        {
            try
            {
                var order = await _unitOfWork.Orders.GetByCodeAsync(request.OrderCode);
                if (order == null)
                {
                    _logger.LogWarning("Order {OrderCode} not found", request.OrderCode);
                    throw new Exception($"Order {request.OrderCode} not found");
                }

                Container container = null;
                if (!string.IsNullOrEmpty(request.ContainerCode))
                {
                    container = await _unitOfWork.Containers.GetByCodeAsync(request.ContainerCode);
                    if (container == null)
                    {
                        throw new Exception($"Container {request.ContainerCode} not found");
                    }
                }

                // 3. Generate OrderDetailId
                var orderDetailId = await GenerateOrderDetailIdAsync();

                // 4. Calculate SubTotal
                decimal? subTotal = null;
                if (request.Price.HasValue && !string.IsNullOrEmpty(request.Quantity))
                {
                    if (int.TryParse(request.Quantity, out var qty))
                    {
                        subTotal = request.Price.Value * qty;
                    }
                }

                // 5. Create OrderDetail
                var orderDetail = new OrderDetail
                {
                    OrderDetailId = orderDetailId,
                    OrderCode = request.OrderCode,
                    StorageCode = request.StorageCode,
                    ContainerCode = request.ContainerCode,
                    Price = request.Price,
                    Quantity = request.Quantity,
                    SubTotal = subTotal,
                    Address = request.Address,
                    Image = request.Image
                };

                await _unitOfWork.OrderDetails.AddAsync(orderDetail);

                if (request.ProductTypeIds != null && request.ProductTypeIds.Any())
                {
                    foreach (var productTypeId in request.ProductTypeIds)
                    {
                        var orderDetailProductType = new OrderDetailProductType
                        {
                            OrderDetailId = orderDetailId,
                            ProductTypeId = productTypeId,
                            IsActive = true
                        };
                        await _unitOfWork.OrderDetailProductTypes.AddAsync(orderDetailProductType);
                    }
                }

                if (request.ServiceIds != null && request.ServiceIds.Any())
                {
                    foreach (var serviceId in request.ServiceIds)
                    {
                        var orderDetailService = new ASMS.Repositories.Entities.OrderDetailService
                        {
                            OrderDetailId = orderDetailId,
                            ServiceId = serviceId
                        };
                        await _unitOfWork.OrderDetailServices.AddAsync(orderDetailService);
                    }
                }



                await _unitOfWork.CompleteAsync();

                _logger.LogInformation("Order detail {DetailId} created successfully", orderDetailId);

                return new CreateOrderDetailResponse
                {
                    OrderDetailId = orderDetailId,
                    OrderCode = request.OrderCode,
                    StorageCode = request.StorageCode,
                    ContainerCode = request.ContainerCode,
                    FloorCode = container?.FloorCode,
                    FloorNumber = container?.FloorCodeNavigation?.FloorNumber,
                    Price = request.Price,
                    Quantity = request.Quantity,
                    SubTotal = subTotal,
                    Address = request.Address,
                    Image = request.Image,
                    Status = string.IsNullOrEmpty(request.ContainerCode) ? "Pending" : "Assigned"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order detail");
                throw;
            }
        }
        // Lấy order details
        public async Task<List<CreateOrderDetailResponse>> GetOrderDetailsAsync(string orderCode)
        {
            var orderDetails = await _unitOfWork.OrderDetails.GetByOrderCodeAsync(orderCode);

            return orderDetails.Select(od => new CreateOrderDetailResponse
            {
                OrderDetailId = od.OrderDetailId,
                OrderCode = od.OrderCode,
                StorageCode = od.StorageCode,
                ContainerCode = od.ContainerCode,
                FloorCode = od.ContainerCodeNavigation?.FloorCode,
                FloorNumber = null,
                //ServiceId = od.ServiceId,
                Price = od.Price,
                Quantity = od.Quantity,
                SubTotal = od.SubTotal,
                Address = od.Address,
                Image = od.Image,
                Status = "Assigned"
            }).ToList();
        }
        // Generate order code theo format: YYYYMMDD-XXXX
        private async Task<string> GenerateOrderCodeAsync(DateOnly date)
        {
            var dateStr = date.ToString("yyyyMMdd");
            var count = await _unitOfWork.Orders.CountOrdersByDateAsync(date);
            var sequence = (count + 1).ToString("D4");

            return $"{dateStr}-{sequence}";
        }

        // Generate order detail id (tự động tăng)
        private async Task<int> GenerateOrderDetailIdAsync()
        {
            var maxId = await _unitOfWork.OrderDetails.GetMaxOrderDetailIdAsync();
            return maxId + 1;
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
