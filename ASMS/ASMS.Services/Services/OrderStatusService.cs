using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASMS.Repositories.Entities;
using ASMS.Repositories.Infrastructures;
using ASMS.Services.Interfaces;
using ASMS.Services.Model.OrderStatus;
using Microsoft.Extensions.Logging;

namespace ASMS.Services.Services
{
    public class OrderStatusService : IOrderStatusService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<OrderStatusService> _logger;

        // Định nghĩa quy trình trạng thái cho từng Style (tất cả lowercase)
        private readonly Dictionary<string, List<string>> _workflowsByStyle = new()
{
    {
        "full", new List<string>  
        {
            "new", "pending", "wait pick up", "verify", "checkout",
            "pick up", "processing", "stored", "retrieved"
        }
    },
    {
        "self_with_delivery", new List<string> 
        {
            "new", "pending", "wait pick up", "verify", "checkout",
            "pick up", "renting", "retrieved"
        }
    },
    {
        "self_no_delivery", new List<string>  
        {
            "new", "pending", "checkout", "renting", "retrieved"
        }
    }
};

        public OrderStatusService(IUnitOfWork unitOfWork, ILogger<OrderStatusService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        #region Public Methods

        /// <summary>
        /// Kiểm tra và cập nhật tự động các đơn hàng quá hạn
        /// </summary>
        public async Task CheckAndUpdateOverdueOrdersAsync()
        {
            try
            {
                var currentDate = DateOnly.FromDateTime(DateTime.Now);
                var overdueOrders = await _unitOfWork.Orders.GetOverdueOrdersAsync(currentDate);

                foreach (var order in overdueOrders)
                {
                    var currentStatus = order.Status?.ToLower();
                    if (currentStatus == "stored" || currentStatus == "renting")
                    {
                        var oldStatus = order.Status;
                        order.Status = "overdue";

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
        public async Task<OrderStatusResponse?> ExtendOrderAsync(string orderCode, DateOnly newReturnDate)
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

                // Nếu đơn hàng đang Overdue, chuyển về trạng thái trước đó
                if (currentStatus == "overdue")
                {
                    // Xác định trạng thái trước Overdue dựa vào Style
                    if (order.Style?.ToLower() == "full" || order.Style?.ToLower() == "self")
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
                if (order.Status?.ToLower() != "overdue")
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
                    Message = "Order status retrieved successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting order status for {orderCode}");
                throw;
            }
        }

        #endregion

        #region Private Helper Methods

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
            return status?.ToLower() switch
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
                "overdue" => "Order Overdue",
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

            // Lấy CurrentAssign từ NextAssign của tracking history gần nhất
            var latestTracking = await _unitOfWork.TrackingHistories.GetLatestByOrderCodeAsync(orderCode);
            string? currentAssign = latestTracking?.NextAssign;

            // Nếu là đơn hàng mới hoặc không có tracking history, tìm nhân viên phù hợp
            var newStatusLower = newStatus?.ToLower();
            if (currentAssign == null || newStatusLower == "new" || newStatusLower == "pending")
            {
                currentAssign = await AssignEmployeeBasedOnStatusAsync(newStatus, order.Style, orderCode);
            }

            // Xác định NextAssign cho trạng thái tiếp theo
            var workflow = await GetWorkflowForOrderAsync(order);
            var currentIndex = workflow.IndexOf(newStatusLower ?? "new");
            string? nextAssignEmployee = null;

            if (currentIndex >= 0 && currentIndex < workflow.Count - 1)
            {
                var nextStatus = workflow[currentIndex + 1];
                nextAssignEmployee = await AssignEmployeeBasedOnStatusAsync(nextStatus, order.Style, orderCode);
            }

            var trackingHistory = new TrackingHistory
            {
                OrderCode = orderCode,
                OrderDetailCode = null,
                OldStatus = oldStatus,
                NewStatus = newStatus,
                ActionType = actionType,
                CreateAt = DateOnly.FromDateTime(DateTime.Now),
                CurrentAssign = currentAssign,
                NextAssign = nextAssignEmployee ?? currentAssign,
                Image = null
            };

            await _unitOfWork.TrackingHistories.AddAsync(trackingHistory);
        }

        /// <summary>
        /// Gán nhân viên dựa trên trạng thái và role phù hợp
        /// Nếu role giống với tracking history gần nhất thì giữ nguyên nhân viên đó
        /// </summary>
        private async Task<string?> AssignEmployeeBasedOnStatusAsync(string? status, string? orderStyle, string? orderCode = null)
        {
            if (string.IsNullOrEmpty(status)) return null;

            var statusLower = status.ToLower();
            string roleName = statusLower switch
            {
                "new" => "Manager",
                "pending" or "wait pick up" or "verify" or "checkout" or "pick up" => "Delivery Staff",
                "processing" or "stored" or "renting" => "Warehouse Staff",
                _ => "Warehouse Staff"
            };

            // Kiểm tra xem có nhân viên đang làm order này với cùng role không
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

            // Role khác hoặc không có tracking history trước đó -> tìm nhân viên mới
            var employee = await _unitOfWork.Employee.GetAvailableEmployeeByRoleAsync(roleName);

            if (employee != null)
            {
                if (statusLower != "stored" && statusLower != "renting" && roleName != "Manager")
                {
                    employee.Status = "InOrder";
                    await _unitOfWork.Employee.UpdateAsync(employee);
                }

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

            // ✅ Xác định bước cuối cùng của từng role
            bool isLastStepOfRole = false;

            if (roleName == "Manager" && newStatusLower == "pending")
            {
                isLastStepOfRole = true;
            }
            else if (roleName == "Delivery Staff" && newStatusLower == "pick up")
            {
                isLastStepOfRole = true;
            }
            else if (roleName == "Warehouse Staff" && (newStatusLower == "stored" || newStatusLower == "renting"))
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

        #endregion
    }
}

