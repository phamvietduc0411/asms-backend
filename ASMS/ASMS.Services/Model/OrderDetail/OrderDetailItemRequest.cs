using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Model.OrderDetail
{
    public class OrderDetailItemRequest
    {
        public string? StorageCode { get; set; }

        public string? ContainerCode { get; set; }

        public decimal? Price { get; set; }

        public string? Quantity { get; set; }

        //public string? Address { get; set; }

        public string? Image { get; set; }

        public int? ContainerType { get; set; }

        public int? ContainerQuantity { get; set; }
        public int? StorageTypeId { get; set; }

        public int? ShelfTypeId { get; set; }

        public int? ShelfQuantity { get; set; }
        public bool? IsPlaced { get; set; }
        public decimal? Length { get; set; }

        public decimal? Width { get; set; }

        public decimal? Height { get; set; }

        public List<int>? ProductTypeIds { get; set; }

        public List<int>? ServiceIds { get; set; }
    }
}
