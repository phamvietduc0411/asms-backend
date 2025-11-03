using System;
using System.Collections.Generic;

namespace ASMS.Repositories.Entities;

public partial class Container
{
    public string ContainerCode { get; set; } = null!;

    public string? FloorCode { get; set; }

    public decimal? Length { get; set; }

    public decimal? Width { get; set; }

    public decimal? Height { get; set; }

    public bool? IsActive { get; set; }

    public string? Status { get; set; }

    public decimal? Price { get; set; }

    public int? ProductTypeId { get; set; }

    public int? MaxItems { get; set; }

    public int? CurrentItemCount { get; set; }

    public decimal? MaxWeight { get; set; }

    public decimal? CurrentWeight { get; set; }

    public decimal? UsedVolume { get; set; }

    public decimal? PositionX { get; set; }

    public decimal? PositionY { get; set; }

    public decimal? PositionZ { get; set; }

    public int? RotationAngle { get; set; }

    public bool? HasFragileItems { get; set; }

    public bool? HasHeavyItems { get; set; }

    public DateTime? LastOptimizedDate { get; set; }

    public decimal? OptimizationScore { get; set; }

    public string? Notes { get; set; }

    public decimal? TotalVolume { get; set; }

    public decimal? UtilizationRate { get; set; }

    public virtual ICollection<ContainerLocationLog> ContainerLocationLogs { get; set; } = new List<ContainerLocationLog>();

    public virtual Floor? FloorCodeNavigation { get; set; }

    public virtual ICollection<Item> Items { get; set; } = new List<Item>();

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual ProductType? ProductType { get; set; }
}
