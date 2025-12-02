using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASMS.Repositories.Infrastructures;
using ASMS.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace ASMS.Services.Services
{
    public class OrderMaintenanceService : IOrderMaintenanceService
    {
        private readonly IOrderStatusService _orderStatusService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<OrderMaintenanceService> _logger;

        private const int DAYS_BEFORE_EXPIRED_STORAGE = 3;

        public OrderMaintenanceService(
            IOrderStatusService orderStatusService,
            IUnitOfWork unitOfWork,
            ILogger<OrderMaintenanceService> logger)
        {
            _orderStatusService = orderStatusService;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        /// <summary>
        /// Job 1: Kiểm tra và cập nhật tất cả orders quá hạn
        /// Chạy mỗi ngày lúc 00:00
        /// </summary>
        public async Task CheckAndProcessOverdueOrdersAsync()
        {
            try
            {
                _logger.LogInformation("Starting CheckAndProcessOverdueOrdersAsync job");

                await _orderStatusService.CheckAndUpdateOverdueOrdersAsync();

                _logger.LogInformation("CheckAndProcessOverdueOrdersAsync job completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CheckAndProcessOverdueOrdersAsync job");
                throw;
            }
        }

        /// <summary>
        /// Job 2: Tự động move orders đã overdue quá 3 ngày vào expired storage
        /// Chạy mỗi ngày lúc 01:00
        /// </summary>
        public async Task MoveOldOverdueOrdersToExpiredStorageAsync()
        {
            try
            {
                _logger.LogInformation("Starting MoveOldOverdueOrdersToExpiredStorageAsync job");

    
                var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                var currentDate = DateOnly.FromDateTime(
                    TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone)
                );

   
                var cutoffDate = currentDate.AddDays(-DAYS_BEFORE_EXPIRED_STORAGE);

                _logger.LogInformation($"Checking for overdue orders before {cutoffDate}");

                var overdueOrders = await _unitOfWork.Orders.GetByStatusAsync("overdue");

                int movedCount = 0;
                int failedCount = 0;

                foreach (var order in overdueOrders)
                {
                    try
                    {

                        if (order.ReturnDate.HasValue && order.ReturnDate.Value < cutoffDate)
                        {
                            var daysOverdue = currentDate.DayNumber - order.ReturnDate.Value.DayNumber;

                            _logger.LogInformation(
                                $"Order {order.OrderCode} is {daysOverdue} days overdue. Moving to expired storage..."
                            );


                            var success = await _orderStatusService.MoveToExpiredStorageAsync(order.OrderCode);

                            if (success)
                            {
                                movedCount++;
                                _logger.LogInformation($"Successfully moved order {order.OrderCode} to expired storage");
                            }
                            else
                            {
                                failedCount++;
                                _logger.LogWarning($"Failed to move order {order.OrderCode} to expired storage");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        failedCount++;
                        _logger.LogError(ex, $"Error moving order {order.OrderCode} to expired storage");
                    }
                }

                _logger.LogInformation(
                    $"MoveOldOverdueOrdersToExpiredStorageAsync job completed. " +
                    $"Moved: {movedCount}, Failed: {failedCount}"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in MoveOldOverdueOrdersToExpiredStorageAsync job");
                throw;
            }
        }
    }
}
