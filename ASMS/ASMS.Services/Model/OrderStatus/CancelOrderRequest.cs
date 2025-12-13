using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Model.OrderStatus
{
    public class CancelOrderRequest
    {
        public string OrderCode { get; set; } = null!;
        public string? CancelReason { get; set; }
    }
}
