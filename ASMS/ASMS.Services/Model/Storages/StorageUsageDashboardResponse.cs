using ASMS.Repositories.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Model.Storages
{
    public class StorageUsageDashboardResponse
    {
        public string StorageCode { get; set; }
        public string StorageTypeName { get; set; }
        public decimal TotalVolume { get; set; }
        public decimal UsedVolume { get; set; }
        public decimal PercentUsed { get; set; }
        public decimal PercentRemaining { get; set; }

    }
}
