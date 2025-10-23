using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Model.Orders
{
    public class OrderResponse
    {
        public string OrderCode { get; set; }
        public string? CustomerCode { get; set; }
        public DateOnly? OrderDate { get; set; }
        public DateOnly? DepositDate { get; set; }
        public DateOnly? ReturnDate { get; set; }
        public string? Status { get; set; }
        public string? PaymentStatus { get; set; }
        public decimal? TotalPrice { get; set; }
        public decimal? UnpaidAmount { get; set; }
    }
}
