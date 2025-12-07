using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Model.Pricing
{
    public class ShippingRateResponse
    {
        public int ShippingRateId { get; set; }
        public decimal DistanceMinKm { get; set; }
        public decimal? DistanceMaxKm { get; set; }
        public int ContainerQtyMin { get; set; }
        public int? ContainerQtyMax { get; set; }
        public decimal BasePrice { get; set; }
        public string PriceUnit { get; set; } = null!;
        public decimal? SpecialItemSurcharge { get; set; }
        public decimal? MonthlyRentalDiscount { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool? IsActive { get; set; }
        public string? DistanceRangeDisplay { get; set; }
        public string? ContainerQtyDisplay { get; set; }
    }
}
