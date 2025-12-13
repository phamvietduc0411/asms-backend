using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Model.Dashboard
{
    public class StorageTypeUsageDetail
    {
        public int StorageTypeId { get; set; }
        public string StorageTypeName { get; set; }
        public int TotalRooms { get; set; }
        public int OccupiedRooms { get; set; }
        public int AvailableRooms { get; set; }
        public decimal PercentUsed { get; set; }
        public decimal PercentRemaining { get; set; }
    }
}
