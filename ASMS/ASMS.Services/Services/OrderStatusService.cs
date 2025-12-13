using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ASMS.Repositories.Entities;
using ASMS.Repositories.Infrastructures;
using ASMS.Services.Interfaces;
using ASMS.Services.Model.Authentication;
using ASMS.Services.Model.OrderStatus;
using ASMS.Services.Model.TrackingHistories;
using ASMS.Services.Utilities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ASMS.Services.Services
{
    public class OrderStatusService : IOrderStatusService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<OrderStatusService> _logger;
        private readonly IPasswordService _passwordService;
        private readonly ProjectMailConfig _mailConfig;

        // Định nghĩa quy trình trạng thái cho từng Style (tất cả lowercase)
        private readonly Dictionary<string, List<string>> _workflowsByStyle = new()
{
    {
        "full", new List<string>
        {
            "pending", "wait pick up", "verify", "checkout",
            "pick up", "delivered", "processing", "stored", "retrieved",
            "wait pick up", "pick up", "delivered", "completed" 
        }
    },
    {
        "self_with_delivery", new List<string>
        {
            "pending", "wait pick up", "verify", "checkout",
            "pick up", "delivered", "processing", "renting", "retrieved",
            "wait pick up", "pick up", "delivered", "completed" 
        }
    },
    {
        "self_no_delivery", new List<string>
        {
            "pending", "checkout", "processing", "renting", "retrieved", "completed" 
        }
    }
};

        public OrderStatusService(IUnitOfWork unitOfWork, ILogger<OrderStatusService> logger, IPasswordService passwordService, IOptions<ProjectMailConfig> mailConfig)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _passwordService = passwordService;
            _mailConfig = mailConfig.Value;
        }

        #region Public Methods

        /// <summary>
        /// Kiểm tra và cập nhật tự động các đơn hàng quá hạn
        /// </summary>
        public async Task CheckAndUpdateOverdueOrdersAsync()
        {
            try
            {
                var currentDate = GetVietnamToday();
                var overdueOrders = await _unitOfWork.Orders.GetOverdueOrdersAsync(currentDate);

                foreach (var order in overdueOrders)
                {
                    var currentStatus = order.Status?.ToLower();
                    if (currentStatus == "stored" || currentStatus == "renting")
                    {
                        int overdueDays = currentDate.DayNumber - (order.ReturnDate?.DayNumber ?? currentDate.DayNumber);

                        var oldStatus = order.Status;
                        order.Status = $"overdue {overdueDays} days";

                        await _unitOfWork.Orders.UpdateAsync(order);

                        // Thêm tracking history
                        await AddTrackingHistoryAsync(
                            orderCode: order.OrderCode,
                            oldStatus: oldStatus,
                            newStatus: "overdue",
                            actionType: "Auto Update - Overdue Detected"
                        );

                        _logger.LogInformation($"Order {order.OrderCode} marked as Overdue");
                    }
                    else if (currentStatus?.StartsWith("overdue") == true)
                    {
                        int overdueDays = currentDate.DayNumber - (order.ReturnDate?.DayNumber ?? currentDate.DayNumber);
                        var newStatus = $"overdue {overdueDays} days";

                        // Chỉ update nếu số ngày thay đổi
                        if (order.Status != newStatus)
                        {
                            var oldStatus = order.Status;
                            order.Status = newStatus;
                            await _unitOfWork.Orders.UpdateAsync(order);

                            _logger.LogInformation($"Order {order.OrderCode} overdue days updated to {overdueDays}");
                        }
                    }
                }

                await _unitOfWork.CompleteAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking overdue orders");
                throw;
            }
        }

        /// <summary>
        /// Cập nhật trạng thái Order theo quy trình tự động
        /// </summary>
        public async Task UpdateOrderStatusAsync(string orderCode)
        {
            try
            {
                var order = await _unitOfWork.Orders.GetByCodeAsync(orderCode);
                if (order == null)
                {
                    throw new InvalidOperationException($"Order {orderCode} not found");
                }

                // Bỏ qua nếu đang ở trạng thái Waiting for Payment
                if (order.Status?.ToLower() == "waiting for payment")
                {
                    _logger.LogWarning($"Order {orderCode} is in 'Waiting for Payment' status. Cannot update to next status.");
                    return;
                }

                // Lấy workflow phù hợp
                var workflow = await GetWorkflowForOrderAsync(order);
                var currentIndex = workflow.IndexOf(order.Status?.ToLower() ?? "new");

                if (currentIndex == -1)
                {
                    if (order.Status?.ToLower() == "store in expired storage")
                    {
                        var previousStatus = order.Status;
                        order.Status = "retrieved";

                        await _unitOfWork.Orders.UpdateAsync(order);

                        var retrievalActionType = "Customer Retrieved from Expired Storage";

                        await AddTrackingHistoryAsync(
                            orderCode: orderCode,
                            oldStatus: previousStatus,
                            newStatus: "retrieved",
                            actionType: retrievalActionType
                        );

                        await _unitOfWork.CompleteAsync();

                        _logger.LogInformation($"Order {orderCode} retrieved from expired storage");

                        // Cập nhật OrderActionCount cho tất cả nhân viên
                        await UpdateEmployeeActionCountsAsync(orderCode);
                        await _unitOfWork.CompleteAsync();

                        return;
                    }

                    throw new InvalidOperationException($"Current status '{order.Status}' not found in workflow");
                }

                // Kiểm tra xem đã đến cuối quy trình chưa
                if (currentIndex >= workflow.Count - 1)
                {
                    _logger.LogInformation($"Order {orderCode} is already at final status: {order.Status}");
                    return;
                }

                var oldStatus = order.Status;
                var newStatus = workflow[currentIndex + 1];

                // Cập nhật trạng thái Order
                order.Status = newStatus;
                await _unitOfWork.Orders.UpdateAsync(order);

                // Xác định ActionType dựa vào trạng thái
                var actionType = GetActionTypeForStatus(newStatus);

                // Thêm tracking history
                await AddTrackingHistoryAsync(
                    orderCode: orderCode,
                    oldStatus: oldStatus,
                    newStatus: newStatus,
                    actionType: actionType
                );

                // Cập nhật trạng thái nhân viên
                await UpdateEmployeeStatusAsync(orderCode, newStatus);
                if (newStatus?.ToLower() == "renting" && order.Style?.ToLower() == "self")
                {
                    try
                    {
                        var passKey = await GenerateUniquePassKeyAsync();  

                        // Cập nhật vào Order
                        order.Passkey = passKey;  
                        await _unitOfWork.Orders.UpdateAsync(order);

                        // Gửi email
                        if (!string.IsNullOrEmpty(order.Email))
                        {
                            await SendPassKeyEmailAsync(order.Email, order.OrderCode, passKey);
                        }

                        _logger.LogInformation($"PassKey generated and sent for order {orderCode}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error generating PassKey for order {orderCode}");
                    }
                }
                if (order.Style?.ToLower() == "self")
                {
                    await UpdateStorageStatusForSelfOrderAsync(orderCode, newStatus);
                }
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation($"Order {orderCode} status updated from {oldStatus} to {newStatus}");
                if (newStatus?.ToLower() == "retrieved")
                {
                    await UpdateEmployeeActionCountsAsync(orderCode);
                    await _unitOfWork.CompleteAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating order status for {orderCode}");
                throw;
            }
        }

        /// <summary>
        /// Gia hạn đơn hàng
        /// </summary>
        public async Task<OrderStatusResponse?> ExtendOrderAsync(string orderCode, DateOnly newReturnDate, decimal unpaidAmount)
        {
            try
            {
                var order = await _unitOfWork.Orders.GetByCodeAsync(orderCode);
                if (order == null)
                {
                    return null;
                }

                var oldReturnDate = order.ReturnDate;
                var oldStatus = order.Status;
                var currentStatus = order.Status?.ToLower();

                order.ReturnDate = newReturnDate;
                order.UnpaidAmount = unpaidAmount;
                order.TotalPrice += unpaidAmount;
                order.PaymentStatus = "Unpaid";

                // Nếu đơn hàng đang Overdue, chuyển về trạng thái trước đó
                if (currentStatus?.StartsWith("overdue") == true)
                {
                    // Xác định trạng thái trước Overdue dựa vào Style
                    if (order.Style?.ToLower() == "self")
                    {
                        order.Status = "renting";
                    }
                    else
                    {
                        order.Status = "stored";
                    }
                }

                await _unitOfWork.Orders.UpdateAsync(order);

                // Thêm tracking history
                await AddTrackingHistoryAsync(
                    orderCode: orderCode,
                    oldStatus: oldStatus,
                    newStatus: order.Status,
                    actionType: $"Order Extended - New Return Date: {newReturnDate}"
                );

                await _unitOfWork.CompleteAsync();

                return new OrderStatusResponse
                {
                    OrderCode = order.OrderCode,
                    Status = order.Status ?? "",
                    PaymentStatus = order.PaymentStatus,
                    Style = order.Style,
                    ReturnDate = order.ReturnDate,
                    BuildingCode = order.BuildingCode,
                    Message = $"Order extended successfully. Old return date: {oldReturnDate}, New return date: {newReturnDate}"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error extending order {orderCode}");
                throw;
            }
        }

        /// <summary>
        /// Chuyển đơn hàng vào kho quá hạn
        /// </summary>
        public async Task<bool> MoveToExpiredStorageAsync(string orderCode)
        {
            try
            {
                var order = await _unitOfWork.Orders.GetByCodeAsync(orderCode);
                if (order == null)
                {
                    return false;
                }
                if (!order.Status?.ToLower().StartsWith("overdue") == true)
                {
                    _logger.LogWarning($"Order {orderCode} is not overdue (current status: {order.Status}). Cannot move to expired storage.");
                    throw new InvalidOperationException($"Order must be in 'overdue' status to move to expired storage. Current status: {order.Status}");
                }

                // Tìm kho quá hạn
                var expiredWarehouse = await _unitOfWork.Building.GetByNameAsync("WareHouse Expired");
                if (expiredWarehouse == null)
                {
                    throw new InvalidOperationException("Expired warehouse not found in system");
                }

                var oldBuilding = order.BuildingCode;
                var oldStatus = order.Status;

                // Cập nhật Order
                order.BuildingCode = expiredWarehouse.BuildingCode;
                order.Status = "store in expired storage";

                await _unitOfWork.Orders.UpdateAsync(order);

                // Thêm tracking history
                await AddTrackingHistoryAsync(
                    orderCode: orderCode,
                    oldStatus: oldStatus,
                    newStatus: "store in expired storage",
                    actionType: $"Moved to Expired Storage - Building: {expiredWarehouse.BuildingCode}"
                );

                await _unitOfWork.CompleteAsync();

                _logger.LogInformation($"Order {orderCode} moved to expired storage");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error moving order {orderCode} to expired storage");
                throw;
            }
        }

        /// <summary>
        /// Toggle trạng thái thanh toán (Waiting for Payment)
        /// </summary>
        public async Task<OrderStatusResponse?> TogglePaymentStatusAsync(string orderCode)
        {
            try
            {
                var order = await _unitOfWork.Orders.GetByCodeAsync(orderCode);
                if (order == null)
                {
                    return null;
                }

                var oldStatus = order.Status;
                string newStatus;
                string actionType;

                if (order.Status?.ToLower() == "waiting for payment")
                {
                    // Tìm trạng thái trước khi vào Waiting for Payment
                    var previousTracking = await GetPreviousStatusBeforePaymentWaitingAsync(orderCode);

                    if (previousTracking != null)
                    {
                        newStatus = previousTracking.OldStatus ?? "pending";
                    }
                    else
                    {
                        newStatus = "pending"; 
                    }

                    actionType = "Payment Completed - Resume Workflow";
                    order.PaymentStatus = "Paid";
                }
                else
                {
                    // Chuyển sang Waiting for Payment
                    newStatus = "waiting for payment";
                    actionType = "Payment Required";
                    order.PaymentStatus = "Unpaid";
                }

                order.Status = newStatus;
                await _unitOfWork.Orders.UpdateAsync(order);

                // Thêm tracking history
                await AddTrackingHistoryAsync(
                    orderCode: orderCode,
                    oldStatus: oldStatus,
                    newStatus: newStatus,
                    actionType: actionType
                );

                await _unitOfWork.CompleteAsync();

                return new OrderStatusResponse
                {
                    OrderCode = order.OrderCode,
                    Status = order.Status ?? "",
                    PaymentStatus = order.PaymentStatus,
                    Style = order.Style,
                    ReturnDate = order.ReturnDate,
                    BuildingCode = order.BuildingCode,
                    Message = $"Payment status toggled. Status changed from {oldStatus} to {newStatus}"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error toggling payment status for order {orderCode}");
                throw;
            }
        }

        /// <summary>
        /// Lấy thông tin trạng thái Order hiện tại
        /// </summary>
        public async Task<OrderStatusResponse?> GetOrderStatusAsync(string orderCode)
        {
            try
            {
                var order = await _unitOfWork.Orders.GetByCodeAsync(orderCode);
                if (order == null)
                {
                    return null;
                }

                var latestTracking = await _unitOfWork.TrackingHistories.GetLatestByOrderCodeAsync(orderCode);

                return new OrderStatusResponse
                {
                    OrderCode = order.OrderCode,
                    Status = order.Status ?? "",
                    PaymentStatus = order.PaymentStatus,
                    Style = order.Style,
                    ReturnDate = order.ReturnDate,
                    CurrentAssignedEmployee = latestTracking?.CurrentAssign,
                    BuildingCode = order.BuildingCode,
                    DepositDate = order.DepositDate,
                    TotalPrice = order.TotalPrice,
                    Message = "Order status retrieved successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting order status for {orderCode}");
                throw;
            }
        }

        /// <summary>
        /// Cập nhật image cho tracking history mới nhất của đơn hàng
        /// </summary>
        public async Task<TrackingHistoryResponse?> UpdateLatestTrackingImageAsync(UpdateTrackingImageRequest request)
        {
            try
            {
                var latestTracking = await _unitOfWork.TrackingHistories.GetLatestByOrderCodeAsync(request.OrderCode);
                if (latestTracking == null)
                {
                    _logger.LogWarning($"No tracking history found for order {request.OrderCode}");
                    return null;
                }

                var trackingToUpdate = await _unitOfWork.TrackingHistories.GetEntityByIdAsync(latestTracking.TrackingHistoryId);
                if (trackingToUpdate == null)
                {
                    _logger.LogWarning($"Tracking history {latestTracking.TrackingHistoryId} not found");
                    return null;
                }

                string? imageJson = null;
                if (request.Image != null && request.Image.Any())
                {
                    try
                    {
                        imageJson = JsonSerializer.Serialize(request.Image);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, $"Error serializing ImageUrls for tracking history of order {request.OrderCode}");
                    }
                }


                trackingToUpdate.Image = imageJson;

                await _unitOfWork.TrackingHistories.UpdateAsync(trackingToUpdate);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation($"Updated tracking history image for order {request.OrderCode}");

                // Bước 6: Deserialize lại để trả về response
                List<string>? imageUrls = new List<string>();
                if (!string.IsNullOrEmpty(trackingToUpdate.Image))
                {
                    try
                    {
                        imageUrls = JsonSerializer.Deserialize<List<string>>(trackingToUpdate.Image);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, $"Error deserializing ImageUrls for response");
                        imageUrls = new List<string>();
                    }
                }

                // Bước 7: Tạo response thủ công
                return new TrackingHistoryResponse
                {
                    TrackingHistoryId = trackingToUpdate.TrackingHistoryId,
                    OrderCode = trackingToUpdate.OrderCode,
                    OrderDetailCode = trackingToUpdate.OrderDetailCode,
                    OldStatus = trackingToUpdate.OldStatus,
                    NewStatus = trackingToUpdate.NewStatus,
                    ActionType = trackingToUpdate.ActionType,
                    CurrentAssign = trackingToUpdate.CurrentAssign,
                    NextAssign = trackingToUpdate.NextAssign,
                    Image = imageUrls,
                    CreateAt = trackingToUpdate.CreateAt
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating tracking image for order {request.OrderCode}");
                throw;
            }
        }
        /// <summary>
        /// Cập nhật PassKey cho Self order
        /// </summary>
        public async Task<UpdatePassKeyResponse> UpdatePassKeyAsync(UpdatePassKeyRequest request)
        {
            try
            {
                if (request.NewPassKey < 100000 || request.NewPassKey > 999999)
                {
                    return new UpdatePassKeyResponse
                    {
                        Success = false,
                        Message = "PassKey mới phải là số có 6 chữ số (từ 100000 đến 999999)"
                    };
                }

                var order = await _unitOfWork.Orders.GetByCodeAsync(request.OrderCode);
                if (order == null)
                {
                    return new UpdatePassKeyResponse
                    {
                        Success = false,
                        Message = $"Đơn hàng {request.OrderCode} không tồn tại"
                    };
                }

                if (order.Style?.ToLower() != "self")
                {
                    return new UpdatePassKeyResponse
                    {
                        Success = false,
                        Message = "Chỉ đơn hàng Self-Storage mới có PassKey"
                    };
                }

                if (order.Passkey != request.OldPassKey)
                {
                    return new UpdatePassKeyResponse
                    {
                        Success = false,
                        Message = "PassKey cũ không đúng"
                    };
                }

                var existingOrder = await _unitOfWork.Orders.GetByPassKeyAsync(request.NewPassKey);
                if (existingOrder != null && existingOrder.OrderCode != request.OrderCode)
                {
                    return new UpdatePassKeyResponse
                    {
                        Success = false,
                        Message = "PassKey mới đã được sử dụng bởi đơn hàng khác. Vui lòng chọn PassKey khác"
                    };
                }

                // Cập nhật PassKey
                order.Passkey = request.NewPassKey;
                await _unitOfWork.Orders.UpdateAsync(order);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation($"PassKey updated for order {request.OrderCode}");

                return new UpdatePassKeyResponse
                {
                    Success = true,
                    Message = "Cập nhật PassKey thành công",
                    OrderCode = request.OrderCode,
                    NewPassKey = request.NewPassKey
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating PassKey for order {request.OrderCode}");
                return new UpdatePassKeyResponse
                {
                    Success = false,
                    Message = $"Lỗi cập nhật PassKey: {ex.Message}"
                };
            }
        }
        /// <summary>
        /// Cập nhật Refund (tiền đền bù hư hại) cho Order
        /// </summary>
        public async Task<UpdateRefundResponse> UpdateRefundAsync(UpdateRefundRequest request)
        {
            try
            {
                if (request.Refund < 0)
                {
                    return new UpdateRefundResponse
                    {
                        Success = false,
                        Message = "Số tiền đền bù không được âm"
                    };
                }

                var order = await _unitOfWork.Orders.GetByCodeAsync(request.OrderCode);
                if (order == null)
                {
                    return new UpdateRefundResponse
                    {
                        Success = false,
                        Message = $"Đơn hàng {request.OrderCode} không tồn tại"
                    };
                }

                order.Refund = request.Refund;

                //order.TotalPrice = (order.TotalPrice ?? 0) + request.Refund;
                //order.UnpaidAmount = (order.UnpaidAmount ?? 0) + request.Refund;

                await _unitOfWork.Orders.UpdateAsync(order);
                await _unitOfWork.CompleteAsync();

                _logger.LogInformation($"Refund updated for order {request.OrderCode}: {request.Refund}");

                return new UpdateRefundResponse
                {
                    Success = true,
                    Message = "Cập nhật tiền đền bù thành công",
                    OrderCode = request.OrderCode,
                    Refund = order.Refund,
                    TotalPrice = order.TotalPrice,
                    UnpaidAmount = order.UnpaidAmount
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating refund for order {request.OrderCode}");
                return new UpdateRefundResponse
                {
                    Success = false,
                    Message = $"Lỗi cập nhật tiền đền bù: {ex.Message}"
                };
            }
        }
        /// <summary>
        /// Hủy đơn hàng (chỉ cho phép khi status = pending)
        /// </summary>
        public async Task<CancelOrderResponse> CancelOrderAsync(CancelOrderRequest request)
        {
            try
            {
                var order = await _unitOfWork.Orders.GetByCodeAsync(request.OrderCode);
                if (order == null)
                {
                    return new CancelOrderResponse
                    {
                        Success = false,
                        Message = $"Đơn hàng {request.OrderCode} không tồn tại"
                    };
                }

                // Chỉ cho phép hủy khi status = pending
                if (order.Status?.ToLower() != "pending")
                {
                    return new CancelOrderResponse
                    {
                        Success = false,
                        Message = $"Chỉ có thể hủy đơn hàng ở trạng thái 'pending'. Trạng thái hiện tại: {order.Status}"
                    };
                }

                var oldStatus = order.Status;
                order.Status = "cancelled";

                await _unitOfWork.Orders.UpdateAsync(order);

                // Thêm tracking history
                string actionType = string.IsNullOrEmpty(request.CancelReason)
                    ? "Order Cancelled"
                    : $"Order Cancelled - Reason: {request.CancelReason}";

                await AddTrackingHistoryAsync(
                    orderCode: request.OrderCode,
                    oldStatus: oldStatus,
                    newStatus: "cancelled",
                    actionType: actionType
                );

                // Nếu là Self order, release storages
                if (order.Style?.ToLower() == "self")
                {
                    try
                    {
                        await ReleaseStoragesForCancelledOrderAsync(request.OrderCode);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error releasing storages for cancelled order {request.OrderCode}");
                    }
                }

                // Nếu là Full order, restore container type quantity
                if (order.Style?.ToLower() == "full")
                {
                    try
                    {
                        await RestoreContainerTypeQuantityForCancelledOrderAsync(request.OrderCode);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error restoring container type quantity for cancelled order {request.OrderCode}");
                    }
                }

                await _unitOfWork.CompleteAsync();

                _logger.LogInformation($"Order {request.OrderCode} cancelled successfully");

                return new CancelOrderResponse
                {
                    Success = true,
                    Message = "Hủy đơn hàng thành công",
                    OrderCode = request.OrderCode,
                    OldStatus = oldStatus,
                    NewStatus = "cancelled"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error cancelling order {request.OrderCode}");
                return new CancelOrderResponse
                {
                    Success = false,
                    Message = $"Lỗi hủy đơn hàng: {ex.Message}"
                };
            }
        }

        #endregion

        #region Private Helper Methods
        /// <summary>
        /// Release storages khi hủy Self order
        /// </summary>
        private async Task ReleaseStoragesForCancelledOrderAsync(string orderCode)
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
                    var storage = await _unitOfWork.Storages.GetByCodeAsync(storageCode);
                    if (storage == null) continue;

                    // Chỉ release nếu status = Reserved
                    if (storage.Status?.ToLower() == "reserved")
                    {
                        storage.Status = "Ready";
                        await _unitOfWork.Storages.UpdateAsync(storage);
                        _logger.LogInformation($"Storage {storageCode} released for cancelled order {orderCode}");
                    }
                }

                await _unitOfWork.CompleteAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error releasing storages for cancelled order {orderCode}");
                throw;
            }
        }
        /// <summary>
        /// Restore ContainerType quantity khi hủy Full order
        /// </summary>
        private async Task RestoreContainerTypeQuantityForCancelledOrderAsync(string orderCode)
        {
            try
            {
                var orderDetails = await _unitOfWork.OrderDetails.GetByOrderCodeAsync(orderCode);

                foreach (var detail in orderDetails)
                {
                    if (detail.ContainerType == null || detail.ContainerQuantity == null)
                        continue;

                    var containerType = await _unitOfWork.ContainerType.GetByIdAsync(detail.ContainerType.Value);
                    if (containerType == null) continue;

                    // Lấy ProductTypeIds để xác định AC hay Normal
                    var productTypes = await _unitOfWork.OrderDetailProductTypes.GetByOrderDetailIdAsync(detail.OrderDetailId);
                    bool needAC = productTypes.Any(pt => pt.ProductTypeId == 3);

                    int quantityRestore = detail.ContainerQuantity.Value;

                    if (needAC)
                    {
                        containerType.AvailableQuantityInAc = (containerType.AvailableQuantityInAc ?? 0) + quantityRestore;
                    }
                    else
                    {
                        containerType.AvailableQuantityInNor = (containerType.AvailableQuantityInNor ?? 0) + quantityRestore;
                    }

                    await _unitOfWork.ContainerType.UpdateAsync(containerType);
                    _logger.LogInformation($"Restored {quantityRestore} containers for type {detail.ContainerType} (cancelled order {orderCode})");
                }

                await _unitOfWork.CompleteAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error restoring container type quantity for cancelled order {orderCode}");
                throw;
            }
        }

        /// <summary>
        /// Lấy workflow phù hợp cho Order dựa vào Style và có Delivery hay không
        /// </summary>
        private async Task<List<string>> GetWorkflowForOrderAsync(Order order)
        {
            var style = order.Style?.ToLower();

            if (style == "full")
            {
                return _workflowsByStyle["full"];
            }
            else if (style == "self")
            {
                bool hasDelivery = await HasDeliveryServiceAsync(order);

                if (hasDelivery)
                {
                    return _workflowsByStyle["self_with_delivery"];
                }
                else
                {
                    return _workflowsByStyle["self_no_delivery"];
                }
            }

            return _workflowsByStyle["full"];
        }

        /// <summary>
        /// Xác định ActionType dựa vào trạng thái
        /// </summary>
        private string GetActionTypeForStatus(string status)
        {
            var statusLower = status?.ToLower();

            if (statusLower?.StartsWith("overdue") == true)
            {
                return "Order Overdue";
            }

            return statusLower switch
            {
                "pending" => "Order Created",
                "wait pick up" => "Ready for Pickup",
                "verify" => "Verification",
                "checkout" => "Checkout Completed",
                "pick up" => "Picked Up",
                "processing" => "Processing Order",
                "stored" => "Stored in Warehouse",
                "renting" => "Renting Active",
                "retrieved" => "Order Retrieved",
                "delivered" => "Delivered to Warehouse/Customer",
                "completed" => "Order Completed",
                _ => "Status Update"
            };
        }

        /// <summary>
        /// Thêm tracking history mới
        /// </summary>
        private async Task AddTrackingHistoryAsync(
    string orderCode,
    string? oldStatus,
    string? newStatus,
    string actionType,
    string? nextAssign = null)
        {
            var order = await _unitOfWork.Orders.GetByCodeAsync(orderCode);
            if (order == null) return;

            var latestTracking = await _unitOfWork.TrackingHistories.GetLatestByOrderCodeAsync(orderCode);
            string? currentAssign = latestTracking?.NextAssign;

            if (currentAssign == null)
            {
                currentAssign = await AssignEmployeeBasedOnStatusAsync(newStatus, order.Style, orderCode);
            }

            var workflow = await GetWorkflowForOrderAsync(order);
            var currentIndex = workflow.IndexOf(newStatus?.ToLower() ?? "pending");
            string? nextAssignEmployee = null;

            if (currentIndex >= 0 && currentIndex < workflow.Count - 1)
            {
                var nextStatus = workflow[currentIndex + 1];

                var currentRole = await GetRoleForStatusAsync(newStatus, order);
                var nextRole = await GetRoleForStatusAsync(nextStatus, order);

                if (currentRole != nextRole)
                {
                    nextAssignEmployee = await AssignEmployeeBasedOnStatusAsync(nextStatus, order.Style, orderCode);
                }
                else
                {
                    nextAssignEmployee = currentAssign;
                }
            }

            var trackingHistory = new TrackingHistory
            {
                OrderCode = orderCode,
                OrderDetailCode = null,
                OldStatus = oldStatus,
                NewStatus = newStatus,
                ActionType = actionType,
                CreateAt = GetVietnamToday(),
                CurrentAssign = currentAssign,
                NextAssign = nextAssignEmployee ?? currentAssign,
                Image = null
            };

            await _unitOfWork.TrackingHistories.AddAsync(trackingHistory);
        }
        /// <summary>
        /// Lấy role tương ứng với status
        /// </summary>
        private async Task<string> GetRoleForStatusAsync(string? status, Order order)
        {
            if (string.IsNullOrEmpty(status)) return "Warehouse Staff";

            var statusLower = status.ToLower();

            if (statusLower == "pending")
            {
                var workflow = await GetWorkflowForOrderAsync(order);
                return workflow == _workflowsByStyle["self_no_delivery"]
                    ? "Warehouse Staff"
                    : "Delivery Staff";
            }

            return statusLower switch
            {
                "wait pick up" or "verify" or "checkout" or "pick up" or "delivered" => "Delivery Staff",
                "processing" or "stored" or "renting" => "Warehouse Staff",
                "retrieved" or "completed" => "Warehouse Staff",
                _ => "Warehouse Staff"
            };
        }
        /// <summary>
        /// Gán nhân viên dựa trên trạng thái và role phù hợp
        /// Nếu role giống với tracking history gần nhất thì giữ nguyên nhân viên đó
        /// </summary>
        private async Task<string?> AssignEmployeeBasedOnStatusAsync(string? status, string? orderStyle, string? orderCode = null)
        {
            if (string.IsNullOrEmpty(status)) return null;

            var order = await _unitOfWork.Orders.GetByCodeAsync(orderCode);
            if (order == null) return null;

            var roleName = await GetRoleForStatusAsync(status, order);

            if (!string.IsNullOrEmpty(orderCode))
            {
                var latestTracking = await _unitOfWork.TrackingHistories.GetLatestByOrderCodeAsync(orderCode);
                if (latestTracking?.CurrentAssign != null)
                {
                    var currentEmployee = await _unitOfWork.Employee.GetByCodeAsync(latestTracking.CurrentAssign);
                    if (currentEmployee?.EmployeeRole?.Name == roleName)
                    {
                        return currentEmployee.EmployeeCode;
                    }
                }
            }

            var employee = await _unitOfWork.Employee.GetAvailableEmployeeByRoleAsync(roleName);
            if (employee != null)
            {
                //var statusLower = status.ToLower();
                //if (statusLower != "stored" && statusLower != "renting")
                //{
                //    employee.Status = "InOrder";
                //    await _unitOfWork.Employee.UpdateAsync(employee);
                //}
                return employee.EmployeeCode;
            }

            return null;
        }

        /// <summary>
        /// Cập nhật trạng thái nhân viên dựa trên trạng thái Order
        /// </summary>
        private async Task UpdateEmployeeStatusAsync(string orderCode, string newStatus)
        {
            var latestTracking = await _unitOfWork.TrackingHistories.GetLatestByOrderCodeAsync(orderCode);

            if (latestTracking?.CurrentAssign == null) return;

            var employee = await _unitOfWork.Employee.GetByCodeAsync(latestTracking.CurrentAssign);

            if (employee == null) return;

            var newStatusLower = newStatus?.ToLower();
            var roleName = employee.EmployeeRole?.Name;

            bool isLastStepOfRole = false;

            if (roleName == "Delivery Staff" && newStatusLower == "delivered")
            {
                var trackingHistories = await _unitOfWork.TrackingHistories.GetByOrderCodeAsync(orderCode);
                var deliveredCount = trackingHistories.Count(th => th.NewStatus?.ToLower() == "delivered");

                if (deliveredCount >= 2) 
                {
                    isLastStepOfRole = true;
                }
            }
            else if (roleName == "Warehouse Staff" && newStatusLower == "completed") 
            {
                isLastStepOfRole = true;
            }

            if (isLastStepOfRole)
            {
                employee.Status = "Active";
                await _unitOfWork.Employee.UpdateAsync(employee);
            }
        }

        /// <summary>
        /// Cập nhật OrderActionCount cho tất cả nhân viên tham gia Order
        /// </summary>
        private async Task UpdateEmployeeActionCountsAsync(string orderCode)
        {
            var trackingHistories = await _unitOfWork.TrackingHistories.GetByOrderCodeAsync(orderCode);

            // Lấy danh sách nhân viên đã tham gia (distinct)
            var employeeCodes = trackingHistories
                .Where(th => !string.IsNullOrEmpty(th.CurrentAssign))
                .Select(th => th.CurrentAssign)
                .Distinct()
                .ToList();

            foreach (var employeeCode in employeeCodes)
            {
                if (string.IsNullOrEmpty(employeeCode)) continue;

                var employee = await _unitOfWork.Employee.GetByCodeAsync(employeeCode);
                if (employee != null)
                {
                    employee.OrderActionCount = (employee.OrderActionCount ?? 0) + 1;
                    employee.Status = "Active"; // Đặt lại về Active khi hoàn thành
                    await _unitOfWork.Employee.UpdateAsync(employee);
                }
            }
        }

        /// <summary>
        /// Tìm trạng thái trước khi vào Waiting for Payment
        /// </summary>
        private async Task<TrackingHistory?> GetPreviousStatusBeforePaymentWaitingAsync(string orderCode)
        {
            var trackingHistories = await _unitOfWork.TrackingHistories.GetByOrderCodeAsync(orderCode);

            return trackingHistories
                .Where(th => th.NewStatus?.ToLower() == "waiting for payment")
                .OrderByDescending(th => th.CreateAt)
                .ThenByDescending(th => th.TrackingHistoryId)
                .FirstOrDefault();
        }

        /// <summary>
        /// Kiểm tra xem Order có service Delivery không
        /// </summary>
        private async Task<bool> HasDeliveryServiceAsync(Order order)
        {
            var orderDetails = await _unitOfWork.OrderDetails.GetByOrderCodeAsync(order.OrderCode);

            foreach (var orderDetail in orderDetails)
            {
                var orderDetailServices = await _unitOfWork.OrderDetailServices.GetByOrderDetailIdAsync(orderDetail.OrderDetailId);

                foreach (var ods in orderDetailServices)
                {
                    var service = await _unitOfWork.Services.GetByIdAsync(ods.ServiceId);
                    if (service?.Name?.ToLower() == "delivery")
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        /// <summary>
        /// Update Storage status theo workflow của Self order
        /// </summary>
        private async Task UpdateStorageStatusForSelfOrderAsync(string orderCode, string newStatus)
        {
            try
            {
                var orderDetails = await _unitOfWork.OrderDetails.GetByOrderCodeAsync(orderCode);
                var storageCodes = orderDetails
                    .Where(od => !string.IsNullOrEmpty(od.StorageCode))
                    .Select(od => od.StorageCode)
                    .Distinct()
                    .ToList();

                var newStatusLower = newStatus?.ToLower();
                string? storageStatus = null;

                switch (newStatusLower)
                {
                    case "renting":
                        storageStatus = "Rented";
                        break;
                    case "retrieved":
                    case "completed":
                        storageStatus = "Ready";
                        break;
                }

                if (storageStatus == null) return;

                foreach (var storageCode in storageCodes)
                {
                    var storage = await _unitOfWork.Storages.GetByCodeAsync(storageCode);
                    if (storage == null)
                    {
                        _logger.LogWarning($"Storage {storageCode} not found for order {orderCode}");
                        continue;
                    }

                    storage.Status = storageStatus;
                    await _unitOfWork.Storages.UpdateAsync(storage);

                    _logger.LogInformation($"Storage {storageCode} status updated to {storageStatus} for order {orderCode}");
                }

                await _unitOfWork.CompleteAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating storage status for order {orderCode}");
                throw;
            }
        }
        /// <summary>
        /// Tạo PassKey ngẫu nhiên 6 chữ số không trùng
        /// </summary>
        private async Task<int> GenerateUniquePassKeyAsync()
        {
            var random = new Random();
            int passKey;
            bool isUnique;

            do
            {
                passKey = random.Next(100000, 999999); 

                // Check trùng với order chưa completed
                var existingOrder = await _unitOfWork.Orders.GetByPassKeyAsync(passKey);
                isUnique = existingOrder == null || existingOrder.Status?.ToLower() == "completed";

            } while (!isUnique);

            return passKey;
        }

        /// <summary>
        /// Gửi PassKey qua email
        /// </summary>
        private async Task<bool> SendPassKeyEmailAsync(string email, string orderCode, int passKey)
        {
            try
            {
                if (string.IsNullOrEmpty(email)) return false;

                string emailContent = EmailTemplates.OrderPassKey(orderCode, passKey.ToString(), _mailConfig.Email);
                await _passwordService.SendEmailAsync(
                    email,
                    $"Mã truy cập Self Storage - Đơn hàng {orderCode}",
                    emailContent);

                _logger.LogInformation($"PassKey email sent to {email} for order {orderCode}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending PassKey email for order {orderCode}");
                return false;
            }
        }
        /// <summary>
        /// Lấy ngày hiện tại theo múi giờ Việt Nam (UTC+7)
        /// </summary>
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

        #endregion
    }
}

