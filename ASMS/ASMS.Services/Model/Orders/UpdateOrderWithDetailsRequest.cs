using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Model.Orders
{
    public class UpdateOrderWithDetailsRequest
    {
        public DateOnly? DepositDate { get; set; }
        public DateOnly? ReturnDate { get; set; }
        public string? Status { get; set; }
        public string? PaymentStatus { get; set; }
        public decimal? TotalPrice { get; set; }
        public decimal? UnpaidAmount { get; set; }
        public string? CustomerName { get; set; }
        public string? PhoneContact { get; set; }
        public string? Email { get; set; }
        public string? Note { get; set; }
        public List<string>? Image { get; set; }
        public string? Address { get; set; }
        public string? Style { get; set; }
        public int? StorageTypeId { get; set; }
        public int? ShelfTypeId { get; set; }
        public int? ShelfQuantity { get; set; }

        public List<UpdateOrderDetailItemRequest> OrderDetails { get; set; } = new();
    }
}
