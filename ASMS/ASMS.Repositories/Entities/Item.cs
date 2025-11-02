using System;
using System.Collections.Generic;

namespace ASMS.Repositories.Entities;

public partial class Item
{
    public int ItemId { get; set; }

    public int OrderDetailId { get; set; }

    public string? ContainerCode { get; set; }

    public int ProductTypeId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public decimal? Length { get; set; }

    public decimal? Width { get; set; }

    public decimal? Height { get; set; }

    public decimal? Weight { get; set; }

    public decimal? Volume { get; set; }

    public int? Quantity { get; set; }

    public bool? IsFragile { get; set; }

    public bool? CanStack { get; set; }

    public string? FrequencyUse { get; set; }

    public DateTime? LastAccessDate { get; set; }

    public int? AccessCount { get; set; }

    public decimal? PositionX { get; set; }

    public decimal? PositionY { get; set; }

    public decimal? PositionZ { get; set; }

    public int? RotationAngle { get; set; }

    public decimal? PlacementScore { get; set; }

    public string? ImageUrl { get; set; }

    public decimal? EstimatedValue { get; set; }

    public string? Brand { get; set; }

    public string? Notes { get; set; }

    public string? Status { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual Container? ContainerCodeNavigation { get; set; }

    public virtual OrderDetail OrderDetail { get; set; } = null!;

    public virtual ProductType ProductType { get; set; } = null!;
}
