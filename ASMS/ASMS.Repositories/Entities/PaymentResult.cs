using System;
using System.Collections.Generic;

namespace ASMS.Repositories.Entities
{
    public partial class PaymentResult
    {
        public int Id { get; set; }

        public string PaymentCode { get; set; } = null!;

        public string OrderCode { get; set; } = null!;

        public string Status { get; set; } = null!;

        public string Message { get; set; } = null!;

        public string Url { get; set; } = null!;

        public decimal Amount { get; set; }

        public DateTime CreatedAt { get; set; }

        public virtual Order Order { get; set; } = null!;
    }
}
