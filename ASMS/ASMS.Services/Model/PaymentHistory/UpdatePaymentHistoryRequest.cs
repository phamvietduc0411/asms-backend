using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Model.PaymentHistory
{
    public class UpdatePaymentHistoryRequest
    {
        public string? OrderCode { get; set; }
        public string? PaymentMethod { get; set; }
        public string? PaymentPlatform { get; set; }
        public decimal? Amount { get; set; }
    }
}
