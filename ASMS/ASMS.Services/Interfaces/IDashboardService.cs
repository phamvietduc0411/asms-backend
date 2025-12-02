using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Interfaces
{
    public interface IDashboardService
    {
        //  Số lượng Order theo tuần/tháng, phân theo trạng thái ---
        Task<int> GetOrderStatisticsAsync(DateOnly date, string? status, bool isWeekly);

        ////Tổng doanh thu theo tuần / tháng / năm ---
        //Task GetRevenueAsync(DateTime startDate, DateTime endDate);

        ////  Báo cáo hợp đồng
        //Task GetContractReportAsync(DateTime reportDate);

        //// Xuất Excel Payment History
        //Task<byte[]> ExportPaymentHistoryToExcelAsync(string? customerCode, string? orderCode);

        ////  % sử dụng của các loại kho
        //Task GetWarehouseUsagePercentAsync();
    }
}
