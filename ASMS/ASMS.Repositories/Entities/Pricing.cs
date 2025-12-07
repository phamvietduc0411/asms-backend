using System;
using System.Collections.Generic;

namespace ASMS.Repositories.Entities;

public partial class Pricing
{
    public int PricingId { get; set; }

    public string ServiceType { get; set; } = null!;

    public string? ItemCode { get; set; }

    public bool? HasAirConditioning { get; set; }

    public decimal? PricePerMonth { get; set; }

    public decimal? PricePerWeek { get; set; }

    public decimal? PricePerTrip { get; set; }

    public string? AdditionalInfo { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool? IsActive { get; set; }
}
