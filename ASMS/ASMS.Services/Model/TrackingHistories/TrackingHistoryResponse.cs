using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Model.TrackingHistories
{
    public class TrackingHistoryResponse
    {
        public int TrackingHistoryId { get; set; }
        public string? OrderDetailCode { get; set; }
        public string? OldStatus { get; set; }
        public string? NewStatus { get; set; }
        public string? ActionType { get; set; }
        public DateOnly? CreateAt { get; set; }
        public string? CurrentAssign { get; set; }
        public string? NextAssign { get; set; }
        public string? Image { get; set; }
        public string? OrderCode { get; set; }
    }
}
