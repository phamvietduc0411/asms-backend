using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Model.TrackingHistories
{
    public class OrderTrackingFlowResponse
    {
        public string OrderCode { get; set; } = null!;
        public string? CurrentStatus { get; set; }
        public List<TrackingHistoryResponse> TrackingFlow { get; set; } = new();
        public int TotalSteps { get; set; }
    }
}
