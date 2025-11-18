using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Model.Orders
{
    public class CreateOrderRequest
    {

        [Required]
        [StringLength(50)]
        public string CustomerCode { get; set; }

        public DateOnly? OrderDate { get; set; }
        public DateOnly? DepositDate { get; set; }
        public DateOnly? ReturnDate { get; set; }

        [StringLength(20)]
        public string? Status { get; set; }

        [StringLength(20)]
        public string? PaymentStatus { get; set; }

    }
}
