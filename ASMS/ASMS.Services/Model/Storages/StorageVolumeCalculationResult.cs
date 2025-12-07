using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Model.Storages
{
    public class StorageVolumeCalculationResult
    {
        public string StorageCode { get; set; }
        public string StorageName { get; set; }
        public string BuildingName { get; set; }
        public string Status { get; set; }
        public decimal TotalVolume { get; set; }
        public decimal UsedVolume { get; set; }
        public decimal UtilizationRate { get; set; }
        public int TotalContainers { get; set; }
        public bool IsReserved { get; set; }
        public string CalculationMethod { get; set; }
    }
}
