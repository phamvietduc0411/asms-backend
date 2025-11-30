using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Model.OrderStatus
{
    public class ExtendOrderRequest
    {
        public string OrderCode { get; set; } = null!;
        public DateOnly NewReturnDate { get; set; }
    }
}
