using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Model.OrderDetail
{
    public class OrderDetailResponse
    {
        public int OrderDetailId { get; set; }
        public string? OrderCode { get; set; }
        public string? StorageCode { get; set; }
        public string? ContainerCode { get; set; }
        public int? ServiceId { get; set; }
        public decimal? Price { get; set; }
        public string? Quantity { get; set; }
        public decimal? SubTotal { get; set; }
        public string? Address { get; set; }
        public string? Image { get; set; }
    }
}
