using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Model.Pricing
{
    public class CreateShippingRateRequest
    {
        public decimal DistanceMinKm { get; set; }
        public decimal? DistanceMaxKm { get; set; }
        public int ContainerQtyMin { get; set; }
        public int? ContainerQtyMax { get; set; }
        public decimal BasePrice { get; set; }
        public string PriceUnit { get; set; } = "Fixed";
        public decimal? SpecialItemSurcharge { get; set; } = 0.15m;
        public decimal? MonthlyRentalDiscount { get; set; } = 0.10m;
    }
}
