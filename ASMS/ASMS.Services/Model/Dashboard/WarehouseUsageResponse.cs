using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Model.Dashboard
{
    public class WarehouseUsageResponse
    {
        public List<BuildingUsageResponse> Buildings { get; set; } = new();
        public OverallUsageSummary OverallSummary { get; set; }
    }
}
