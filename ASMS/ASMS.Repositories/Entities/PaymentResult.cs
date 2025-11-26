using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Repositories.Entities
{
    public partial class PaymentResult
    {
        public int Id { get; set; }

        public string PaymentCode { get; set; } = null!;

        public string OrderCode { get; set; } = null!;

        public string Status { get; set; } = null!;

        public string? Message { get; set; }

        public string? Url { get; set; }

        public decimal Amount { get; set; }

        public DateTime CreatedAt { get; set; }

        // Navigation
        public virtual Order Order { get; set; } = null!;
    }
}
