using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Model.PayOS
{
    public class TestWebhookRequest
    {
        public string PaymentCode { get; set; } = null!;
        public string OrderCode { get; set; } = null!;
        public bool IsSuccess { get; set; }
        public decimal Amount { get; set; }
    }
}
