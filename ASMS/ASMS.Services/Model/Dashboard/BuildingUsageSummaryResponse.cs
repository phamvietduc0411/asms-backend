using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Model.Dashboard
{
    public class BuildingUsageSummaryResponse
    {
        public int BuildingId { get; set; }
        public string BuildingName { get; set; }
        public string BuildingCode { get; set; }
        public string BuildingType { get; set; } // "Self-Storage" or "WareHouse"

        // Cho WareHouse
        public int? TotalStorages { get; set; }
        public decimal? TotalVolume { get; set; }
        public decimal? UsedVolume { get; set; }
        public decimal? PercentUsed { get; set; }
        public decimal? PercentRemaining { get; set; }

        // Cho Self-Storage (chi tiết theo StorageType)
        public List<StorageTypeUsageDetail>? StorageTypeDetails { get; set; }
        public StorageTypeSummary? StorageTypeSummary { get; set; }
    }
}
