using System;
using System.Collections.Generic;

namespace ASMS.Repositories.Entities;

public partial class ShippingRate
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
}
