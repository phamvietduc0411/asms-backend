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
        public string ContainerCode { get; set; }
        public string StorageCode { get; set; }
        public decimal? Price { get; set; }
        public string? Quantity { get; set; }
        public string? Address { get; set; }
        public string? Image { get; set; }

        public List<int> ProductTypeIds { get; set; } = new List<int>();
        public List<int> ServiceIds { get; set; } = new List<int>();

    }
}
