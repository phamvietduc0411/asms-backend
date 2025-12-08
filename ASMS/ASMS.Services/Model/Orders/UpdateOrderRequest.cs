using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Model.Orders
{
    public class UpdateOrderRequest
    {
        [StringLength(50)]
        public string? CustomerCode { get; set; }

        public DateOnly? OrderDate { get; set; }
        public DateOnly? DepositDate { get; set; }
        public DateOnly? ReturnDate { get; set; }
        //public string? Status { get; set; }


        //public string? PaymentStatus { get; set; }

        public decimal? TotalPrice { get; set; }
        public decimal? UnpaidAmount { get; set; }


        public string? CustomerName { get; set; }

        public string? PhoneContact { get; set; }

        public string? Email { get; set; }

        public string? Note { get; set; }

        public string? Address { get; set; }

        public List<string>? Image { get; set; }
    }
}
