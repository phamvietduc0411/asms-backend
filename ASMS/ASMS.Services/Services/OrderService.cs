using System.Text.Json;
using ASMS.Repositories.Entities;
using ASMS.Repositories.Infrastructures;
using ASMS.Services.Interfaces;
using ASMS.Services.Model.Authentication;
using ASMS.Services.Model.Customer;
using ASMS.Services.Model.OrderDetail;
using ASMS.Services.Model.Orders;
using ASMS.Services.Model.TrackingHistories;
using ASMS.Services.Utilities;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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
        private readonly IEmployeeService _employeeService;
        private readonly ITrackingHistoryService _trackingHistoryService;

        public OrderService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<OrderService> logger, ICLPService clpService, ICustomerService cusService, IPasswordService password, IOptions<ProjectMailConfig> mailConfig, IEmployeeService employeeService, ITrackingHistoryService trackingHistoryService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _clpService = clpService;
            _cusService = cusService;
            _password = password;
            _mailConfig = mailConfig.Value;
            _employeeService = employeeService;
            _trackingHistoryService = trackingHistoryService;
        }

        public async Task<PaginatedOrderResponse> GetWithFilterAsync(int pageNumber, int pageSize, string? customerCode, DateOnly? orderDate, DateOnly? depositDate, DateOnly? returnDate, string style)
        {
            var orders = await _unitOfWork.Orders.GetWithFilterAsync(pageNumber, pageSize, customerCode, orderDate, depositDate, returnDate, style);
            var totalCount = await _unitOfWork.Orders.GetTotalCountWithFilterAsync(customerCode, orderDate, depositDate, returnDate, style);
            var orderResponses = orders.Select(order =>
            {
                var response = _mapper.Map<OrderResponse>(order);

                if (!string.IsNullOrEmpty(order.Image))
                {
                    try
                    {
                        response.ImageUrls = JsonSerializer.Deserialize<List<string>>(order.Image);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, $"Error deserializing ImageUrls for order {order.OrderCode}");
                        response.ImageUrls = new List<string>();
                    }
                }
                else
                {
                    response.ImageUrls = new List<string>();
                }

                return response;
            }).ToList();
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
            if (order == null) return null;

            var response = _mapper.Map<OrderResponse>(order);

            if (!string.IsNullOrEmpty(order.Image))
            {
                try
                {
                    response.ImageUrls = JsonSerializer.Deserialize<List<string>>(order.Image);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, $"Error deserializing ImageUrls for order {orderCode}");
                    response.ImageUrls = new List<string>();
                }
            }
            else
            {
                response.ImageUrls = new List<string>();
            }

            return response;
        }

        public async Task<CreateOrderResponse> CreateAsync(CreateOrderRequest request)
        {
            _logger.LogInformation("Creating new order for customer {Code}", request.CustomerCode);

            var orderDate = GetVietnamToday();
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

            var orderDate = GetVietnamToday();
            var orderCode = await GenerateOrderCodeAsync(orderDate);

            var existingCustomer = await _unitOfWork.Customer.GetCustomerByEmailAsync(request.Email);
            if (existingCustomer == null)
            {
                request.CustomerCode = await CreateCustomer(request);
                var isCreateSuccess = CreatePasswordAndSendEmail(request.Email);
            }
            else
            {
                request.CustomerCode = existingCustomer.CustomerCode;
            }
            string? imageJson = null;
            if (request.ImageUrls != null && request.ImageUrls.Any())
            {
                try
                {
                    imageJson = JsonSerializer.Serialize(request.ImageUrls);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error serializing ImageUrls for order creation");
                }
            }

            var order = new Order
            {
                OrderCode = orderCode,
                CustomerCode = request.CustomerCode,
                OrderDate = orderDate,
                DepositDate = request.DepositDate,
                ReturnDate = request.ReturnDate,
                Status = string.IsNullOrWhiteSpace(request.Status) ? "pending" : request.Status.ToLower(),
                PaymentStatus = request.PaymentStatus ?? "Unpaid",
                TotalPrice = request.TotalPrice,
                UnpaidAmount = request.UnpaidAmount,
                CustomerName = request.CustomerName,
                PhoneContact = request.PhoneContact,
                Email = request.Email,
                Note = request.Note,
                Image = imageJson,
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

                decimal? subTotal = null;
                if (detailRequest.Price.HasValue && !string.IsNullOrEmpty(detailRequest.Quantity))
                {
                    if (int.TryParse(detailRequest.Quantity, out var qty))
                    {
                        subTotal = detailRequest.Price.Value * qty;
                    }
                }

                var orderDetail = new OrderDetail
                {
                    OrderDetailId = orderDetailId,
                    OrderCode = orderCode,
                    StorageCode = detailRequest.StorageCode,
                    ContainerCode = detailRequest.ContainerCode,
                    Price = detailRequest.Price,
                    Quantity = detailRequest.Quantity,
                    SubTotal = subTotal,
                    StorageTypeId = detailRequest.StorageTypeId,
                    ShelfTypeId = detailRequest.ShelfTypeId,
                    ShelfQuantity = detailRequest.ShelfQuantity,
                    Image = detailRequest.Image,
                    ContainerType = detailRequest.ContainerType,
                    ContainerQuantity = detailRequest.ContainerQuantity,
                    IsPlaced = isPlacedValue,
                    Length = detailRequest.Length,
                    Width = detailRequest.Width,
                    Height = detailRequest.Height,
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
                    StorageTypeId = detailRequest.StorageTypeId,
                    ShelfTypeId = detailRequest.ShelfTypeId,
                    ShelfQuantity = detailRequest.ShelfQuantity,
                    Image = detailRequest.Image,
                    ContainerType = detailRequest.ContainerType,
                    ContainerQuantity = detailRequest.ContainerQuantity,
                    IsPlaced = isPlacedValue,
                    Length = detailRequest.Length,
                    Width = detailRequest.Width,
                    Height = detailRequest.Height,
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

            await CreateInitialTrackingHistoryAsync(orderCode, request.Style);

            if (request.Style?.ToLower() == "self")
            {
                try
                {
                    await ReserveStoragesForSelfOrderAsync(orderCode);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error reserving storages for order {orderCode}, but order created successfully");
                }
            }

            _logger.LogInformation("Order {OrderCode} with {Count} details created successfully", orderCode, orderDetailResponses.Count);

            await SendInvoiceToCustomerAsync(order, orderDetailsToAdd, order.Email);

            return new CreateOrderWithDetailsResponse
            {
                OrderCode = orderCode,
                CustomerCode = request.CustomerCode,
                OrderDate = orderDate,
                DepositDate = request.DepositDate,
                ReturnDate = request.ReturnDate,
                Status = order.Status,
                PaymentStatus = order.PaymentStatus,
                TotalPrice = order.TotalPrice,
                UnpaidAmount = order.UnpaidAmount,
                StorageTypeId = request.StorageTypeId,
                ShelfTypeId = request.ShelfTypeId,
                ShelfQuantity = request.ShelfQuantity,
                CustomerName = request.CustomerName,
                PhoneContact = request.PhoneContact,
                Email = request.Email,
                Note = request.Note,
                ImageUrls = request.ImageUrls,
                Address = request.Address,
                OrderDetails = orderDetailResponses
            };
        }

        public async Task<UpdateOrderWithDetailsResponse> UpdateOrderWithDetailsAsync(string orderCode, UpdateOrderWithDetailsRequest request)
        {
            _logger.LogInformation("Updating order {OrderCode} with details", orderCode);

            var existingOrder = await _unitOfWork.Orders.GetByCodeAsync(orderCode);
            if (existingOrder == null)
            {
                throw new Exception($"Order {orderCode} not found");
            }

            var oldStyle = existingOrder.Style?.ToLower();
            var newStyle = request.Style?.ToLower();
            string? imageJson = null;
            if (request.ImageUrls != null && request.ImageUrls.Any())
            {
                try
                {
                    imageJson = JsonSerializer.Serialize(request.ImageUrls);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, $"Error serializing ImageUrls for order {orderCode}");
                }
            }

            existingOrder.DepositDate = request.DepositDate;
            existingOrder.ReturnDate = request.ReturnDate;
            existingOrder.Status = string.IsNullOrWhiteSpace(request.Status) ? existingOrder.Status : request.Status.ToLower();
            existingOrder.PaymentStatus = request.PaymentStatus ?? existingOrder.PaymentStatus;
            existingOrder.TotalPrice = request.TotalPrice;
            existingOrder.UnpaidAmount = request.UnpaidAmount;
            existingOrder.CustomerName = request.CustomerName;
            existingOrder.PhoneContact = request.PhoneContact;
            existingOrder.Email = request.Email;
            existingOrder.Note = request.Note;
            existingOrder.Image = imageJson;
            existingOrder.Address = request.Address;
            existingOrder.Style = request.Style;

            await _unitOfWork.Orders.UpdateAsync(existingOrder);
            await _unitOfWork.CompleteAsync();

            _logger.LogInformation("Order {OrderCode} updated, now updating order details", orderCode);

            var existingDetails = await _unitOfWork.OrderDetails.GetByOrderCodeAsync(orderCode);
            var existingDetailIds = existingDetails.Select(od => od.OrderDetailId).ToHashSet();

            var requestDetailIds = request.OrderDetails
                .Where(d => d.OrderDetailId.HasValue)
                .Select(d => d.OrderDetailId.Value)
                .ToHashSet();

            var detailsToDelete = existingDetails
                .Where(od => !requestDetailIds.Contains(od.OrderDetailId))
                .ToList();

            foreach (var detailToDelete in detailsToDelete)
            {
                var productTypes = await _unitOfWork.OrderDetailProductTypes.GetByOrderDetailIdForDeleteAsync(detailToDelete.OrderDetailId);
                _unitOfWork.OrderDetailProductTypes.RemoveRange(productTypes);

                var services = await _unitOfWork.OrderDetailServices.GetByOrderDetailIdForDeleteAsync(detailToDelete.OrderDetailId);
                _unitOfWork.OrderDetailServices.RemoveRange(services);

                _unitOfWork.OrderDetails.Remove(detailToDelete);
            }

            await _unitOfWork.CompleteAsync();

            var orderDetailResponses = new List<OrderDetailItemResponse>();
            bool? isPlacedValue = DetermineIsPlacedByStyle(request.Style);

            var baseOrderDetailId = await GenerateOrderDetailIdAsync();
            int newDetailCounter = 0;

            var productTypesToAdd = new List<OrderDetailProductType>();
            var servicesToAdd = new List<ASMS.Repositories.Entities.OrderDetailService>();

            foreach (var detailRequest in request.OrderDetails)
            {
                OrderDetail orderDetail;
                int orderDetailId;

                if (detailRequest.OrderDetailId.HasValue && existingDetailIds.Contains(detailRequest.OrderDetailId.Value))
                {
                    orderDetailId = detailRequest.OrderDetailId.Value;
                    orderDetail = existingDetails.First(od => od.OrderDetailId == orderDetailId);

                    orderDetail.StorageCode = detailRequest.StorageCode;
                    orderDetail.ContainerCode = detailRequest.ContainerCode;
                    orderDetail.Price = detailRequest.Price;
                    orderDetail.Quantity = detailRequest.Quantity;
                    orderDetail.StorageTypeId = detailRequest.StorageTypeId;
                    orderDetail.ShelfTypeId = detailRequest.ShelfTypeId;
                    orderDetail.ShelfQuantity = detailRequest.ShelfQuantity;
                    orderDetail.Image = detailRequest.Image;
                    orderDetail.ContainerType = detailRequest.ContainerType;
                    orderDetail.ContainerQuantity = detailRequest.ContainerQuantity;
                    orderDetail.IsPlaced = isPlacedValue;
                    orderDetail.Length = detailRequest.Length;
                    orderDetail.Width = detailRequest.Width;
                    orderDetail.Height = detailRequest.Height;

                    if (detailRequest.Price.HasValue && !string.IsNullOrEmpty(detailRequest.Quantity))
                    {
                        if (int.TryParse(detailRequest.Quantity, out var qty))
                        {
                            orderDetail.SubTotal = detailRequest.Price.Value * qty;
                        }
                    }

                    await _unitOfWork.OrderDetails.UpdateAsync(orderDetail);
                    await _unitOfWork.CompleteAsync();

                    await _unitOfWork.OrderDetailProductTypes.DeleteByOrderDetailIdAsync(orderDetailId);
                    await _unitOfWork.OrderDetailServices.DeleteByOrderDetailIdAsync(orderDetailId);

                    await _unitOfWork.CompleteAsync();

                    _unitOfWork.Context.ChangeTracker.Clear();

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
                }
                else
                {
                    orderDetailId = baseOrderDetailId + newDetailCounter;
                    newDetailCounter++;

                    decimal? subTotal = null;
                    if (detailRequest.Price.HasValue && !string.IsNullOrEmpty(detailRequest.Quantity))
                    {
                        if (int.TryParse(detailRequest.Quantity, out var qty))
                        {
                            subTotal = detailRequest.Price.Value * qty;
                        }
                    }

                    orderDetail = new OrderDetail
                    {
                        OrderDetailId = orderDetailId,
                        OrderCode = orderCode,
                        StorageCode = detailRequest.StorageCode,
                        ContainerCode = detailRequest.ContainerCode,
                        Price = detailRequest.Price,
                        Quantity = detailRequest.Quantity,
                        SubTotal = subTotal,
                        StorageTypeId = detailRequest.StorageTypeId,
                        ShelfTypeId = detailRequest.ShelfTypeId,
                        ShelfQuantity = detailRequest.ShelfQuantity,
                        Image = detailRequest.Image,
                        ContainerType = detailRequest.ContainerType,
                        ContainerQuantity = detailRequest.ContainerQuantity,
                        IsPlaced = isPlacedValue,
                        Length = detailRequest.Length,
                        Width = detailRequest.Width,
                        Height = detailRequest.Height,
                    };

                    await _unitOfWork.OrderDetails.AddAsync(orderDetail);
                    await _unitOfWork.CompleteAsync();

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
                }

                Container? container = null;
                if (!string.IsNullOrEmpty(detailRequest.ContainerCode))
                {
                    container = await _unitOfWork.Containers.GetByCodeAsync(detailRequest.ContainerCode);
                }

                orderDetailResponses.Add(new OrderDetailItemResponse
                {
                    OrderDetailId = orderDetailId,
                    StorageCode = detailRequest.StorageCode,
                    ContainerCode = detailRequest.ContainerCode,
                    FloorCode = container?.FloorCode,
                    FloorNumber = container?.FloorCodeNavigation?.FloorNumber,
                    Price = detailRequest.Price,
                    Quantity = detailRequest.Quantity,
                    SubTotal = orderDetail.SubTotal,
                    StorageTypeId = detailRequest.StorageTypeId,
                    ShelfTypeId = detailRequest.ShelfTypeId,
                    ShelfQuantity = detailRequest.ShelfQuantity,
                    Image = detailRequest.Image,
                    ContainerType = detailRequest.ContainerType,
                    ContainerQuantity = detailRequest.ContainerQuantity,
                    IsPlaced = isPlacedValue,
                    Length = detailRequest.Length,
                    Width = detailRequest.Width,
                    Height = detailRequest.Height,
                });
            }

            foreach (var productType in productTypesToAdd)
            {
                await _unitOfWork.OrderDetailProductTypes.AddAsync(productType);
            }

            foreach (var service in servicesToAdd)
            {
                await _unitOfWork.OrderDetailServices.AddAsync(service);
            }

            await _unitOfWork.CompleteAsync();

            if (newStyle == "self")
            {
                try
                {
                    await UpdateStoragesForSelfOrderAsync(orderCode, oldStyle);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error updating storages for order {orderCode}");
                }
            }
            else if (oldStyle == "self" && newStyle != "self")
            {
                try
                {
                    await ReleaseStoragesForOrderAsync(orderCode);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error releasing storages for order {orderCode}");
                }
            }

            _logger.LogInformation("Order {OrderCode} with {Count} details updated successfully", orderCode, orderDetailResponses.Count);

            return new UpdateOrderWithDetailsResponse
            {
                OrderCode = orderCode,
                CustomerCode = existingOrder.CustomerCode,
                OrderDate = existingOrder.OrderDate,
                DepositDate = existingOrder.DepositDate,
                ReturnDate = existingOrder.ReturnDate,
                Status = existingOrder.Status,
                PaymentStatus = existingOrder.PaymentStatus,
                TotalPrice = existingOrder.TotalPrice,
                UnpaidAmount = existingOrder.UnpaidAmount,
                CustomerName = existingOrder.CustomerName,
                PhoneContact = existingOrder.PhoneContact,
                Email = existingOrder.Email,
                Note = existingOrder.Note,
                ImageUrls = request.ImageUrls,
                Address = existingOrder.Address,
                Style = existingOrder.Style,
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
                    IsPlaced = od.IsPlaced,
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

        public async Task<List<OrderResponse>> GetActiveOrdersByEmployeeAsync(string employeeCode)
        {
            var orders = await _unitOfWork.Orders.GetActiveOrdersByEmployeeAsync(employeeCode);
            return _mapper.Map<List<OrderResponse>>(orders);
        }

        /// <summary>
        /// Reserve storages khi tạo Self order (Status: pending → Reserved)
        /// </summary>
        private async Task ReserveStoragesForSelfOrderAsync(string orderCode)
        {
            try
            {
                var orderDetails = await _unitOfWork.OrderDetails.GetByOrderCodeAsync(orderCode);
                var storageCodesToReserve = orderDetails
                    .Where(od => !string.IsNullOrEmpty(od.StorageCode))
                    .Select(od => od.StorageCode)
                    .Distinct()
                    .ToList();

                foreach (var storageCode in storageCodesToReserve)
                {
                    var storage = await _unitOfWork.Storages.GetByCodeWithBuildingAsync(storageCode);
                    if (storage == null)
                    {
                        _logger.LogWarning($"Storage {storageCode} not found for order {orderCode}");
                        continue;
                    }

                    if (!ValidateSelfStorage(storage))
                    {
                        _logger.LogWarning($"Storage {storageCode} validation failed for order {orderCode}");
                        continue;
                    }

                    storage.Status = "Reserved";
                    await _unitOfWork.Storages.UpdateAsync(storage);

                    _logger.LogInformation($"Storage {storageCode} reserved for order {orderCode}");
                }

                await _unitOfWork.CompleteAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error reserving storages for order {orderCode}");
                throw;
            }
        }
        /// <summary>
        /// Release storages khi đổi từ Self sang non-Self
        /// </summary>
        private async Task ReleaseStoragesForOrderAsync(string orderCode)
        {
            try
            {
                var orderDetails = await _unitOfWork.OrderDetails.GetByOrderCodeAsync(orderCode);
                var storageCodes = orderDetails
                    .Where(od => !string.IsNullOrEmpty(od.StorageCode))
                    .Select(od => od.StorageCode)
                    .Distinct()
                    .ToList();

                foreach (var storageCode in storageCodes)
                {
                    var storage = await _unitOfWork.Storages.GetByCodeWithBuildingAsync(storageCode);
                    if (storage == null) continue;

                    storage.Status = "Ready";
                    await _unitOfWork.Storages.UpdateAsync(storage);
                }

                await _unitOfWork.CompleteAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error releasing storages for order {orderCode}");
                throw;
            }
        }

        /// <summary>
        /// Update storages cho Self order khi update
        /// </summary>
        private async Task UpdateStoragesForSelfOrderAsync(string orderCode, string? oldStyle)
        {
            try
            {
                // Lấy storage codes hiện tại
                var currentDetails = await _unitOfWork.OrderDetails.GetByOrderCodeAsync(orderCode);
                var currentStorageCodes = currentDetails
                    .Where(od => !string.IsNullOrEmpty(od.StorageCode))
                    .Select(od => od.StorageCode)
                    .Distinct()
                    .ToHashSet();


                if (oldStyle == "self")
                {
                }

                // Reserve các storage mới
                foreach (var storageCode in currentStorageCodes)
                {
                    var storage = await _unitOfWork.Storages.GetByCodeWithBuildingAsync(storageCode);
                    if (storage == null) continue;

                    if (!ValidateSelfStorage(storage)) continue;

                    if (storage.Status?.ToLower() != "reserved")
                    {
                        storage.Status = "Reserved";
                        await _unitOfWork.Storages.UpdateAsync(storage);
                    }
                }

                await _unitOfWork.CompleteAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating storages for order {orderCode}");
                throw;
            }
        }

        /// <summary>
        /// Validate storage có phù hợp cho Self order không
        /// </summary>
        private bool ValidateSelfStorage(Storage storage)
        {
            if (storage.IsActive != true)
            {
                _logger.LogWarning($"Storage {storage.StorageCode} is not active");
                return false;
            }

            if (storage.BuildingId == null)
            {
                _logger.LogWarning($"Storage {storage.StorageCode} has no building");
                return false;
            }

            var building = storage.Building;
            if (building == null)
            {
                _logger.LogWarning($"Building not loaded for storage {storage.StorageCode}");
                return false;
            }

            if (building.IsActive != true)
            {
                _logger.LogWarning($"Building {building.BuildingCode} is not active");
                return false;
            }

            if (building.Status?.ToLower() != "ready")
            {
                _logger.LogWarning($"Building {building.BuildingCode} status is {building.Status}, not Ready");
                return false;
            }

            var buildingName = building.Name?.ToLower();
            return buildingName == "self-storage" || buildingName == "self-storage with ac";
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
                CreateAt = GetVietnamToday()
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
                //string newPass = _password.GenerateRandomPassword(8);
                string newPass = "123456789";
                string emailContent = EmailTemplates.NewAccount(email, newPass, _mailConfig.Email);
                await _password.SendEmailAsync(email, " Kích hoạt tài khoản: Thông tin đăng nhập của bạn", emailContent);
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

            if (normalizedStyle == "self")
                return null;

            return false;
        }

        private async Task AssignDeliveryForOrder(string oderCode)
        {
            var deliveryEmp = await _employeeService.GetDevliveryEmployeeForOder();
            if (deliveryEmp == null) return;
            var assign = new TrackingHistory()
            {
                OrderCode = oderCode,
                OldStatus = "Order created successfully",
                NewStatus = "Waiting for pick up",
                ActionType = "Pending",
                CreateAt = GetVietnamToday(),
                CurrentAssign = deliveryEmp.Name,
                NextAssign = "Warehouse Staff"
            };

            await _trackingHistoryService.CreateAsync(assign);
        }

        private async Task CreateInitialTrackingHistoryAsync(string orderCode, string style)
        {
            try
            {
                var workflow = DetermineWorkflowType(style);
                string? firstEmployee = null;

                if (workflow == "full" || workflow == "self_with_delivery")
                {
                    var deliveryStaff = await _unitOfWork.Employee.GetAvailableEmployeeByRoleAsync("Delivery Staff");
                    if (deliveryStaff != null)
                    {
                        firstEmployee = deliveryStaff.EmployeeCode;

                        deliveryStaff.OrderActionCount = (deliveryStaff.OrderActionCount ?? 0) + 1;
                        await _unitOfWork.Employee.UpdateAsync(deliveryStaff);
                        await _unitOfWork.CompleteAsync();
                    }
                }
                else if (workflow == "self_no_delivery")
                {
                    var warehouseStaff = await _unitOfWork.Employee.GetAvailableEmployeeByRoleAsync("Warehouse Staff");
                    if (warehouseStaff != null)
                    {
                        firstEmployee = warehouseStaff.EmployeeCode;

                        warehouseStaff.OrderActionCount = (warehouseStaff.OrderActionCount ?? 0) + 1;
                        await _unitOfWork.Employee.UpdateAsync(warehouseStaff);
                        await _unitOfWork.CompleteAsync();
                    }
                }

                if (firstEmployee == null)
                {
                    _logger.LogWarning($"No available employee for order {orderCode}, creating tracking without assignment");
                }

                var initialTracking = new TrackingHistory
                {
                    OrderCode = orderCode,
                    OrderDetailCode = null,
                    OldStatus = null,
                    NewStatus = "pending",
                    ActionType = "Order Created",
                    CreateAt = GetVietnamToday(),
                    CurrentAssign = firstEmployee,
                    NextAssign = firstEmployee,
                    Image = null
                };

                await _unitOfWork.TrackingHistories.AddAsync(initialTracking);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation($"Initial tracking history created for order {orderCode}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating initial tracking history for order {orderCode}");
                throw;
            }
        }

        private string DetermineWorkflowType(string? style)
        {
            if (string.IsNullOrEmpty(style)) return "full";

            var normalizedStyle = style.Trim().ToLower();

            if (normalizedStyle == "full") return "full";

            return "self_with_delivery";
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
                    // Fallback: tạo UTC+7 manual
                    var vietnamNow = DateTime.UtcNow.AddHours(7);
                    return DateOnly.FromDateTime(vietnamNow);
                }
            }
        }

        private async Task<bool> SendInvoiceToCustomerAsync(Order order, List<OrderDetail> details, string customerMail)
        {
            try
            {
                var fullOrder = await _unitOfWork.Orders.GetFullOrder(order.OrderCode);
                var html = EmailTemplates.OrderInvoice(fullOrder, _mailConfig.Email);

                await _password.SendEmailAsync(customerMail,
                                                   $"Đơn hàng {order.OrderCode} đã được tạo",
                                                   html);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending invoice email to customer for order {order.OrderCode}");
                return false;
            }
        }

    }
}
