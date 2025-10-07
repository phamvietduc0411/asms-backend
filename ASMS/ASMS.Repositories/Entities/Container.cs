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

    public virtual ICollection<ContainerLocationLog> ContainerLocationLogs { get; set; } = new List<ContainerLocationLog>();

    public virtual Floor? FloorCodeNavigation { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
