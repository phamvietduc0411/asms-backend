using ASMS.Services.Model.Dashboard;
using ASMS.Services.Model.Storages;
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
        Task<OrderStatisticsResponse> GetOrderStatisticsAsync(DateOnly targetDate, string? status, string type);

        //Tổng doanh thu theo tuần / tháng / năm ---
        Task<RevenueResponse> GetRevenueAsync(DateOnly targetDate, string type);

        ////  Báo cáo hợp đồng
        //Task GetContractReportAsync(DateTime reportDate);

        ////  % sử dụng của các loại kho
        Task<List<StorageUsageDashboardResponse>> GetWarehouseUsagePercentAsync();

        Task<BuildingsUsageResponse> GetBuildingUsageSummaryAsync();
    }
}
