using ASMS.Repositories.Infrastructures;
using ASMS.Services.Interfaces;
using ASMS.Services.Utilities;
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






    }
}
