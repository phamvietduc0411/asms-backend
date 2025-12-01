using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Model.OrderStatus
{
    public class OrderStatusResponse
    {
        public string OrderCode { get; set; } = null!;
        public string Status { get; set; } = null!;
        public string? PaymentStatus { get; set; }
        public string? Style { get; set; }
        public DateOnly? ReturnDate { get; set; }
        public string? CurrentAssignedEmployee { get; set; }
        public string? BuildingCode { get; set; }
        public string Message { get; set; } = null!;
    }
}
