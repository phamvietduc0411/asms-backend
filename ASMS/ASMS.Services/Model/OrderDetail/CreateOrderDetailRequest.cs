using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Model.OrderDetail
{
    public class CreateOrderDetailRequest
    {
        public string OrderCode { get; set; }
        public int? ServiceId { get; set; }
        public decimal? Price { get; set; }
        public string? Quantity { get; set; }
        public string? Address { get; set; }
        public string? Image { get; set; }

        // Package info for CLP (required)
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public decimal Weight { get; set; }
        public int ProductTypeId { get; set; }
        public bool IsFragile { get; set; }
        public int StorageDays { get; set; }
    }
}
