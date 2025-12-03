using ASMS.Repositories.Infrastructures;
using ASMS.Services.Interfaces;
using ASMS.Services.Model.Storages;
using ASMS.Services.Utilities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Services
{
    internal class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;
        public DashboardService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<int> GetOrderStatisticsAsync(DateOnly date, string? status, bool isWeekly)
        {
            int numberOfOrders = 0;
            DateTime start;
            DateTime end;

            if (isWeekly)
            {
                // Tính tuần từ Thứ 2 → Chủ nhật
                (start, end) = GetWeekRange(date.ToDateTime(TimeOnly.MinValue));
                numberOfOrders = await _unitOfWork.Orders.GetNumberOfOrders(start, end, status);
                return numberOfOrders;
            }

            start = new DateTime(date.Year, date.Month, 1);
            end = start.AddMonths(1).AddDays(-1);
            numberOfOrders = await _unitOfWork.Orders.GetNumberOfOrders(start, end, status);
            return numberOfOrders;
        }

        private static (DateTime weekStart, DateTime weekEnd) GetWeekRange(DateTime date)
        {
            int diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
            DateTime weekStart = date.AddDays(-diff).Date;
            DateTime weekEnd = weekStart.AddDays(6);
            return (weekStart, weekEnd);
        }

        public async Task<decimal> GetRevenueAsync(DateOnly targetDate, string type)
        {
            var orders =  _unitOfWork.Orders.GetAllToCaculatePrice();

            DateOnly start;
            DateOnly end;

            switch (type.ToLower())
            {
                case "week":
                    int diff = targetDate.DayOfWeek - DayOfWeek.Monday;
                    if (diff < 0) diff += 7;

                    start = targetDate.AddDays(-diff);
                    end = start.AddDays(6);
                    break;

                case "year":
                    start = new DateOnly(targetDate.Year, 1, 1);
                    end = new DateOnly(targetDate.Year, 12, 31);
                    break;

                default: // month
                    start = new DateOnly(targetDate.Year, targetDate.Month, 1);
                    end = start.AddMonths(1).AddDays(-1);
                    break;
            }
            orders = orders.Where(o => o.OrderDate.Value >= start &&
                                     o.OrderDate.Value <= end);
            decimal revenue = (decimal)await orders.SumAsync(o => o.TotalPrice);

            return revenue;
        }

        public async Task<List<StorageUseageDashboardResponse>> GetWarehouseUsagePercentAsync()
        {
            var storages = await _unitOfWork.Storages.GetAllStorage();

            if (storages == null || storages.Count == 0)
                return new List<StorageUseageDashboardResponse>();

            var result = storages.Select(s => {
                decimal total = s.TotalVolume ?? 0m;
                decimal used = s.UsedVolume ?? 0m;

                return new StorageUseageDashboardResponse
                {
                    StorageTypeName = s.StorageType?.Name ?? "Unknown",
                    TotalVolume = (decimal)total,
                    UsedVolume = (decimal)used,
                    PercentUsed = total > 0 ? Math.Round((decimal)(used / total * 100), 2) : 0,
                    PercentRemaining = total > 0 ? Math.Round((decimal)((total - used) / total * 100), 2) : 0
                };
            }).ToList();

            return result;
        }


    }
}
