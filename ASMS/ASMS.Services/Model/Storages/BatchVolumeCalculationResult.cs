using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Model.Storages
{
    public class BatchVolumeCalculationResult
    {
        public int TotalStorages { get; set; }
        public int UpdatedStorages { get; set; }
        public int FailedStorages { get; set; }
        public List<StorageVolumeCalculationResult> Details { get; set; } = new();
        public List<string> Errors { get; set; } = new();
    }
}
