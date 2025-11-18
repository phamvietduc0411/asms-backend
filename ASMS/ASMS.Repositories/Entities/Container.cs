using System;
using System.Collections.Generic;

namespace ASMS.Repositories.Entities;

public partial class Container
{
    public string ContainerCode { get; set; } = null!;

    public string? FloorCode { get; set; }

    public bool? IsActive { get; set; }

    public string? Status { get; set; }

    public decimal? Price { get; set; }

    public int? ProductTypeId { get; set; }

    public decimal? MaxWeight { get; set; }

    public decimal? CurrentWeight { get; set; }

    public decimal? PositionX { get; set; }

    public decimal? PositionY { get; set; }

    public decimal? PositionZ { get; set; }

    public DateTime? LastOptimizedDate { get; set; }

    public decimal? OptimizationScore { get; set; }

    public string? Notes { get; set; }

    public string? ImageUrl { get; set; }

    public int? ContainerTypeId { get; set; }

    public int? SerialNumber { get; set; }

    public int? Layer { get; set; }

    public string? ContainerAboveCode { get; set; }

    public virtual ICollection<ContainerLocationLog> ContainerLocationLogs { get; set; } = new List<ContainerLocationLog>();

    public virtual ContainerType? ContainerType { get; set; }

    public virtual Floor? FloorCodeNavigation { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual ProductType? ProductType { get; set; }
}
