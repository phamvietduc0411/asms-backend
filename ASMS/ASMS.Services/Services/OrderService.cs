using ASMS.Repositories.Entities;
using ASMS.Repositories.Infrastructures;
using ASMS.Services.Interfaces;
using ASMS.Services.Model.Authentication;
using ASMS.Services.Model.CLP;
using ASMS.Services.Model.Customer;
using ASMS.Services.Model.OrderDetail;
using ASMS.Services.Model.Orders;
using ASMS.Services.Model.TrackingHistories;
using ASMS.Services.Utilities;
using AutoMapper;
using Azure.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<OrderService> _logger;
        private readonly ICLPService _clpService;
        private readonly ICustomerService _cusService;
        private readonly IPasswordService _password;
        private readonly ProjectMailConfig _mailConfig;

        public OrderService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<OrderService> logger, ICLPService clpService, ICustomerService cusService, IPasswordService password, IOptions<ProjectMailConfig> mailConfig)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _clpService = clpService;
            _cusService = cusService;
            _password = password;
            _mailConfig = mailConfig.Value;
        }

        public async Task<PaginatedOrderResponse> GetWithFilterAsync(int pageNumber, int pageSize, string? customerCode, DateOnly? orderDate, DateOnly? depositDate, DateOnly? returnDate, string style)
        {
            var orders = await _unitOfWork.Orders.GetWithFilterAsync(pageNumber, pageSize, customerCode, orderDate, depositDate, returnDate, style);
            var totalCount = await _unitOfWork.Orders.GetTotalCountWithFilterAsync(customerCode, orderDate, depositDate, returnDate, style);

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
                    //Address = request.Address,
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

        public async Task<CreateOrderWithDetailsResponse> CreateOrderWithDetailsAsync(CreateOrderWithDetailsRequest request)
        {
            _logger.LogInformation("Creating new order with details for customer {Code}", request.CustomerCode);

            //Create new customer 
            request.CustomerCode = await CreateCustomer(request);

            // Generate order code
            var orderDate = DateOnly.FromDateTime(DateTime.Now);
            var orderCode = await GenerateOrderCodeAsync(orderDate);
            var isCreateSuccess = CreatePasswordAndSendEmail(request.Email);

            // Calculate total price and unpaid amount
            decimal totalPrice = 0;
            foreach (var detail in request.OrderDetails)
            {
                if (detail.Price.HasValue && !string.IsNullOrEmpty(detail.Quantity))
                {
                    if (int.TryParse(detail.Quantity, out var qty))
                    {
                        totalPrice += detail.Price.Value * qty;
                    }
                }
            }

            // Create Order entity
            var order = new Order
            {
                OrderCode = orderCode,
                CustomerCode = request.CustomerCode,
                OrderDate = orderDate,
                DepositDate = request.DepositDate,
                ReturnDate = request.ReturnDate,
                Status = request.Status ?? "Pending",
                PaymentStatus = request.PaymentStatus ?? "Unpaid",
                TotalPrice = totalPrice,
                UnpaidAmount = totalPrice,
                //StorageTypeId = request.StorageTypeId,
                //ShelfTypeId = request.ShelfTypeId,
                //ShelfQuantity = request.ShelfQuantity,
                CustomerName = request.CustomerName,
                PhoneContact = request.PhoneContact,
                Email = request.Email,
                Note = request.Note,
                Image = request.Image,
                Address = request.Address,
                Style = request.Style
            };

            await _unitOfWork.Orders.AddAsync(order);
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation("Order {OrderCode} created, now creating order details", orderCode);

            var baseOrderDetailId = await GenerateOrderDetailIdAsync();

            var orderDetailResponses = new List<OrderDetailItemResponse>();
            var orderDetailsToAdd = new List<OrderDetail>();
            var productTypesToAdd = new List<OrderDetailProductType>();
            var servicesToAdd = new List<ASMS.Repositories.Entities.OrderDetailService>();

            bool? isPlacedValue = DetermineIsPlacedByStyle(request.Style);
            for (int i = 0; i < request.OrderDetails.Count; i++)
            {
                var detailRequest = request.OrderDetails[i];

                var orderDetailId = baseOrderDetailId + i;

                Container container = null;
                if (!string.IsNullOrEmpty(detailRequest.ContainerCode))
                {
                    container = await _unitOfWork.Containers.GetByCodeAsync(detailRequest.ContainerCode);
                    if (container == null)
                    {
                        _logger.LogWarning("Container {ContainerCode} not found", detailRequest.ContainerCode);
                    }
                }

                // Calculate SubTotal
                decimal? subTotal = null;
                if (detailRequest.Price.HasValue && !string.IsNullOrEmpty(detailRequest.Quantity))
                {
                    if (int.TryParse(detailRequest.Quantity, out var qty))
                    {
                        subTotal = detailRequest.Price.Value * qty;
                    }
                }

                // Create OrderDetail
                var orderDetail = new OrderDetail
                {
                    OrderDetailId = orderDetailId,
                    OrderCode = orderCode,
                    StorageCode = detailRequest.StorageCode,
                    ContainerCode = detailRequest.ContainerCode,
                    Price = detailRequest.Price,
                    Quantity = detailRequest.Quantity,
                    SubTotal = subTotal,
                    //Address = detailRequest.Address,
                    StorageTypeId = detailRequest.StorageTypeId,
                    ShelfTypeId = detailRequest.ShelfTypeId,
                    ShelfQuantity = detailRequest.ShelfQuantity,
                    Image = detailRequest.Image,
                    ContainerType = detailRequest.ContainerType,
                    ContainerQuantity = detailRequest.ContainerQuantity,
                    IsPlaced = isPlacedValue,
                };

                orderDetailsToAdd.Add(orderDetail);


                if (detailRequest.ProductTypeIds != null && detailRequest.ProductTypeIds.Any())
                {
                    foreach (var productTypeId in detailRequest.ProductTypeIds)
                    {
                        productTypesToAdd.Add(new OrderDetailProductType
                        {
                            OrderDetailId = orderDetailId,
                            ProductTypeId = productTypeId,
                            IsActive = true
                        });
                    }
                }

                // Prepare Services
                if (detailRequest.ServiceIds != null && detailRequest.ServiceIds.Any())
                {
                    foreach (var serviceId in detailRequest.ServiceIds)
                    {
                        servicesToAdd.Add(new ASMS.Repositories.Entities.OrderDetailService
                        {
                            OrderDetailId = orderDetailId,
                            ServiceId = serviceId
                        });
                    }
                }

                // Add to response list
                orderDetailResponses.Add(new OrderDetailItemResponse
                {
                    OrderDetailId = orderDetailId,
                    StorageCode = detailRequest.StorageCode,
                    ContainerCode = detailRequest.ContainerCode,
                    FloorCode = container?.FloorCode,
                    FloorNumber = container?.FloorCodeNavigation?.FloorNumber,
                    Price = detailRequest.Price,
                    Quantity = detailRequest.Quantity,
                    SubTotal = subTotal,
                    //Address = detailRequest.Address,
                    StorageTypeId = detailRequest.StorageTypeId,
                    ShelfTypeId = detailRequest.ShelfTypeId,
                    ShelfQuantity = detailRequest.ShelfQuantity,
                    Image = detailRequest.Image,
                    ContainerType = detailRequest.ContainerType,
                    ContainerQuantity = detailRequest.ContainerQuantity,
                    IsPlaced = isPlacedValue,
                    //Status = string.IsNullOrEmpty(detailRequest.ContainerCode) ? "Pending" : "Assigned"
                });
            }

            foreach (var detail in orderDetailsToAdd)
            {
                await _unitOfWork.OrderDetails.AddAsync(detail);
            }
            await _unitOfWork.CompleteAsync();

            foreach (var productType in productTypesToAdd)
            {
                await _unitOfWork.OrderDetailProductTypes.AddAsync(productType);
            }

            foreach (var service in servicesToAdd)
            {
                await _unitOfWork.OrderDetailServices.AddAsync(service);
            }

            await _unitOfWork.CompleteAsync();

            _logger.LogInformation("Order {OrderCode} with {Count} details created successfully", orderCode, orderDetailResponses.Count);

            return new CreateOrderWithDetailsResponse
            {
                OrderCode = orderCode,
                CustomerCode = request.CustomerCode,
                OrderDate = orderDate,
                DepositDate = request.DepositDate,
                ReturnDate = request.ReturnDate,
                Status = order.Status,
                PaymentStatus = order.PaymentStatus,
                TotalPrice = totalPrice,
                UnpaidAmount = totalPrice,
                StorageTypeId = request.StorageTypeId,
                ShelfTypeId = request.ShelfTypeId,
                ShelfQuantity = request.ShelfQuantity,
                CustomerName = request.CustomerName,
                PhoneContact = request.PhoneContact,
                Email = request.Email,
                Note = request.Note,
                Image = request.Image,
                Address = request.Address,
                OrderDetails = orderDetailResponses
            };
        }

        // Lấy order details
        public async Task<List<OrderDetailItemResponse>> GetOrderDetailsAsync(string orderCode)
        {
            var orderDetails = await _unitOfWork.OrderDetails.GetByOrderCodeAsync(orderCode);

            return orderDetails.Select(od => new OrderDetailItemResponse
            {
                OrderDetailId = od.OrderDetailId,
                //OrderCode = od.OrderCode,
                StorageCode = od.StorageCode,
                ContainerCode = od.ContainerCode,
                FloorCode = od.ContainerCodeNavigation?.FloorCode,
                FloorNumber = null,
                //ServiceId = od.ServiceId,
                Price = od.Price,
                Quantity = od.Quantity,
                SubTotal = od.SubTotal,
                //Address = od.Address,
                Image = od.Image,
                ContainerType = od.ContainerType,
                ContainerQuantity = od.ContainerQuantity,
                StorageTypeId = od.StorageTypeId,
                ShelfTypeId = od.ShelfTypeId,
                ShelfQuantity = od.ShelfQuantity,
                ProductTypeNames = od.OrderDetailProductTypes
            .Select(odpt => odpt.ProductType?.Name)
            .Where(name => name != null)
            .ToList(),

                ServiceNames = od.OrderDetailServices
            .Select(ods => ods.Service?.Name)
            .Where(name => name != null)
            .ToList()
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

        public async Task<TrackingHistoryResponse> UpdateOrderProcessAsync(UpdateOrderProcessRequest request)
        {

            var order = await _unitOfWork.Orders.GetByCodeAsync(request.OrderCode);
            if (order == null)
                throw new Exception($"Order {request.OrderCode} not found");

            string oldStatus = order.Status ?? "";


            if (request.ActionByRole == "Delivery" && request.NewStatus == "ProgressTask")
            {
                request.NewStatus = "Ready";
            }


            order.Status = request.NewStatus;
            await _unitOfWork.Orders.UpdateAsync(order);


            var tracking = new TrackingHistory
            {
                OrderCode = request.OrderCode,
                OrderDetailCode = request.OrderDetailCode,
                OldStatus = oldStatus,
                NewStatus = request.NewStatus,
                ActionType = request.ActionType,
                CurrentAssign = request.EmployeeCode,
                NextAssign = request.NextAssign,
                Image = request.Image,
                CreateAt = DateOnly.FromDateTime(DateTime.Now)
            };

            await _unitOfWork.TrackingHistories.AddAsync(tracking);


            await _unitOfWork.CompleteAsync();


            return _mapper.Map<TrackingHistoryResponse>(tracking);
        }

        private async Task<String> CreateCustomer(CreateOrderWithDetailsRequest request)
        {
            if (string.IsNullOrEmpty(request.CustomerCode))
            {
                var newCustomerCode = await _cusService.GetLastRecord();
                request.CustomerCode = newCustomerCode.ToString();
            }

            var newCode = request.CustomerCode;
            var newCus = new CreateCustomerRequest()
            {
                CustomerCode = request.CustomerCode,
                Phone = request.PhoneContact,
                Name = request.CustomerName,
                IsActive = true,
                Address = request.Address,
                Email = request.Email,
                Password = PasswordHasher.HashPassword("123456789")
            };

            await _cusService.AddCustomerAsync(newCus);
            return newCode;
        }

        private async Task<bool> CreatePasswordAndSendEmail(string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email)) return false;
                string newPass = _password.GenerateRandomPassword(8);
                string emailContent = EmailTemplates.NewAccount(email, newPass, _mailConfig.Email);
                await _password.SendEmailAsync(email, newPass,emailContent);
                return true;
            }
            catch (Exception ex)
            {
                return true;
            }
        }
        private bool? DetermineIsPlacedByStyle(string style)
        {
            if (string.IsNullOrEmpty(style))
                return false; 

            var normalizedStyle = style.Trim().ToLower();

            if (normalizedStyle == "full" || normalizedStyle == "self")
                return null;

            return false;
        }
    }
}
