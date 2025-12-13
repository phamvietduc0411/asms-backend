using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASMS.Services.Model.Storages;

namespace ASMS.Services.Model.Dashboard
{
    public class BuildingUsageResponse
    {
        public int BuildingId { get; set; }
        public string BuildingName { get; set; }
        public string BuildingCode { get; set; }
        public int TotalStorages { get; set; }
        public decimal TotalVolume { get; set; }
        public decimal UsedVolume { get; set; }
        public decimal PercentUsed { get; set; }
        public decimal PercentRemaining { get; set; }
        public List<StorageUsageDashboardResponse> StorageDetails { get; set; } = new();
    }
}
