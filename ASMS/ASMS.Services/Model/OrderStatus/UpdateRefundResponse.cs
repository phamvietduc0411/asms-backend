using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Model.OrderStatus
{
    public class UpdateRefundResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = null!;
        public string? OrderCode { get; set; }
        public decimal? Refund { get; set; }
        public decimal? TotalPrice { get; set; }
        public decimal? UnpaidAmount { get; set; }
    }
}
