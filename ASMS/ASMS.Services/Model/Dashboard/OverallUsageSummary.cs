using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Model.Dashboard
{
    public class OverallUsageSummary
    {
        public int TotalBuildings { get; set; }
        public int TotalStorages { get; set; }
        public decimal TotalVolume { get; set; }
        public decimal UsedVolume { get; set; }
        public decimal PercentUsed { get; set; }
        public decimal PercentRemaining { get; set; }
    }
}
