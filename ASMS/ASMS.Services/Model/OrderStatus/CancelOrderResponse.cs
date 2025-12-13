using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Model.OrderStatus
{
    public class CancelOrderResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = null!;
        public string? OrderCode { get; set; }
        public string? OldStatus { get; set; }
        public string? NewStatus { get; set; }
    }
}
