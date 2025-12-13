using ASMS.Repositories.Infrastructures;
using ASMS.Services.Interfaces;
using ASMS.Services.Model.Dashboard;
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

        public async Task<OrderStatisticsResponse> GetOrderStatisticsAsync(
    DateOnly targetDate,
    string? status,
    string type)
        {
            DateOnly start;
            DateOnly end;

            switch (type.ToLower())
            {
                case "day":
                    start = targetDate;
                    end = targetDate;
                    break;
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

            var numberOfOrders = await _unitOfWork.Orders.GetNumberOfOrders(start, end, status);

            return new OrderStatisticsResponse
            {
                Date = targetDate.ToString("yyyy-MM-dd"),
                Type = type,
                StartDate = start,
                EndDate = end,
                Status = status,
                TotalOrders = numberOfOrders
            };
        }

        private static (DateTime weekStart, DateTime weekEnd) GetWeekRange(DateTime date)
        {
            int diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
            DateTime weekStart = date.AddDays(-diff).Date;
            DateTime weekEnd = weekStart.AddDays(6);
            return (weekStart, weekEnd);
        }

        public async Task<RevenueResponse> GetRevenueAsync(DateOnly targetDate, string type)
        {
            var orders = _unitOfWork.Orders.GetAllToCaculatePrice();

            DateOnly start;
            DateOnly end;

            switch (type.ToLower())
            {
                case "day":
                    start = targetDate;
                    end = targetDate;
                    break;
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
            var filteredOrders = await orders
                .Where(o => o.OrderDate.HasValue &&
                           o.OrderDate.Value >= start &&
                           o.OrderDate.Value <= end)
                .ToListAsync();
            var totalPrice = filteredOrders.Sum(o => o.TotalPrice ?? 0);

            var totalRefund = filteredOrders.Sum(o => o.Refund ?? 0);

            var netRevenue = totalPrice - totalRefund;

            return new RevenueResponse
            {
                Date = targetDate.ToString("yyyy-MM-dd"),
                Type = type,
                StartDate = start,
                EndDate = end,
                TotalPrice = totalPrice,
                TotalRefund = totalRefund,
                NetRevenue = netRevenue
            };
        }
        /// <summary>
        /// Get building usage summary
        /// - Self-Storage: Tính theo số lượng phòng, group by StorageType
        /// - WareHouse: Tính theo volume
        /// </summary>
        public async Task<BuildingsUsageResponse> GetBuildingUsageSummaryAsync()
        {
            var storages = await _unitOfWork.Storages.GetAllStorageWithBuilding(asNoTracking: true);

            if (storages == null || storages.Count == 0)
            {
                return new BuildingsUsageResponse
                {
                    Buildings = new List<BuildingUsageSummaryResponse>(),
                    OverallSummary = new OverallUsageSummary()
                };
            }

            // Group by BuildingId
            var buildingSummaries = storages
                .Where(s => s.BuildingId.HasValue)
                .GroupBy(s => s.BuildingId.Value)
                .Select(g =>
                {
                    var firstStorage = g.First();
                    var building = firstStorage.Building;
                    var buildingName = building?.Name ?? "Unknown";

   
                    bool isSelfStorage = buildingName.StartsWith("Self-Storage", StringComparison.OrdinalIgnoreCase);

                    if (isSelfStorage)
                    {
                        // ✅ SELF-STORAGE: Chi tiết theo StorageType
                        var storageTypeDetails = g
                            .Where(s => s.StorageTypeId.HasValue)
                            .GroupBy(s => s.StorageTypeId.Value)
                            .Select(stg =>
                            {
                                var storageType = stg.First().StorageType;
                                var totalRooms = stg.Count();
                                var occupiedRooms = stg.Count(s => s.Status == "Reserved");
                                var availableRooms = totalRooms - occupiedRooms;

                                return new StorageTypeUsageDetail
                                {
                                    StorageTypeId = stg.Key,
                                    StorageTypeName = storageType?.Name ?? "Unknown",
                                    TotalRooms = totalRooms,
                                    OccupiedRooms = occupiedRooms,
                                    AvailableRooms = availableRooms,
                                    PercentUsed = totalRooms > 0
                                        ? Math.Round((decimal)occupiedRooms / totalRooms * 100, 2)
                                        : 0,
                                    PercentRemaining = totalRooms > 0
                                        ? Math.Round((decimal)availableRooms / totalRooms * 100, 2)
                                        : 0
                                };
                            })
                            .OrderBy(st => st.StorageTypeName)
                            .ToList();

                        // Tính tổng summary cho building
                        var totalRooms = storageTypeDetails.Sum(st => st.TotalRooms);
                        var occupiedRooms = storageTypeDetails.Sum(st => st.OccupiedRooms);
                        var availableRooms = storageTypeDetails.Sum(st => st.AvailableRooms);

                        return new BuildingUsageSummaryResponse
                        {
                            BuildingId = g.Key,
                            BuildingName = buildingName,
                            BuildingCode = building?.BuildingCode ?? "Unknown",
                            BuildingType = "Self-Storage",
                            StorageTypeDetails = storageTypeDetails,
                            StorageTypeSummary = new StorageTypeSummary
                            {
                                TotalRooms = totalRooms,
                                OccupiedRooms = occupiedRooms,
                                AvailableRooms = availableRooms,
                                PercentUsed = totalRooms > 0
                                    ? Math.Round((decimal)occupiedRooms / totalRooms * 100, 2)
                                    : 0,
                                PercentRemaining = totalRooms > 0
                                    ? Math.Round((decimal)availableRooms / totalRooms * 100, 2)
                                    : 0
                            }
                        };
                    }
                    else
                    {
                        // ✅ WAREHOUSE: Tính theo volume
                        var totalVolume = g.Sum(s => s.TotalVolume ?? 0);
                        var usedVolume = g.Sum(s => s.UsedVolume ?? 0);

                        return new BuildingUsageSummaryResponse
                        {
                            BuildingId = g.Key,
                            BuildingName = buildingName,
                            BuildingCode = building?.BuildingCode ?? "Unknown",
                            BuildingType = "WareHouse",
                            TotalStorages = g.Count(),
                            TotalVolume = totalVolume,
                            UsedVolume = usedVolume,
                            PercentUsed = totalVolume > 0
                                ? Math.Round(usedVolume / totalVolume * 100, 2)
                                : 0,
                            PercentRemaining = totalVolume > 0
                                ? Math.Round((totalVolume - usedVolume) / totalVolume * 100, 2)
                                : 0
                        };
                    }
                })
                .OrderBy(b => b.BuildingName)
                .ToList();

            // Tính Overall Summary
            var totalVolume = storages.Sum(s => s.TotalVolume ?? 0);
            var totalUsedVolume = storages.Sum(s => s.UsedVolume ?? 0);

            var overallSummary = new OverallUsageSummary
            {
                TotalBuildings = buildingSummaries.Count,
                TotalStorages = storages.Count,
                TotalVolume = totalVolume,
                UsedVolume = totalUsedVolume,
                PercentUsed = totalVolume > 0
                    ? Math.Round(totalUsedVolume / totalVolume * 100, 2)
                    : 0,
                PercentRemaining = totalVolume > 0
                    ? Math.Round((totalVolume - totalUsedVolume) / totalVolume * 100, 2)
                    : 0
            };

            return new BuildingsUsageResponse
            {
                Buildings = buildingSummaries,
                OverallSummary = overallSummary
            };
        }

        /// <summary>
        /// Get warehouse usage (legacy - flat list)
        /// </summary>
        public async Task<List<StorageUsageDashboardResponse>> GetWarehouseUsagePercentAsync()
        {
            var storages = await _unitOfWork.Storages.GetAllStorageWithBuilding(asNoTracking: true);

            if (storages == null || storages.Count == 0)
                return new List<StorageUsageDashboardResponse>();

            var result = storages.Select(s => {
                decimal total = s.TotalVolume ?? 0m;
                decimal used = s.UsedVolume ?? 0m;
                return new StorageUsageDashboardResponse
                {
                    StorageCode = s.StorageCode,
                    StorageTypeName = s.StorageType?.Name ?? "Unknown",
                    TotalVolume = total,
                    UsedVolume = used,
                    PercentUsed = total > 0 ? Math.Round(used / total * 100, 2) : 0,
                    PercentRemaining = total > 0 ? Math.Round((total - used) / total * 100, 2) : 0
                };
            }).ToList();

            return result;
        }


    }
}
