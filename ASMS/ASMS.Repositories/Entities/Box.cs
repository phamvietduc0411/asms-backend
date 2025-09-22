using System;
using System.Collections.Generic;

namespace ASMS.Repositories.Entities;

public partial class Box
{
    public string BoxCode { get; set; } = null!;

    public decimal? Length { get; set; }

    public decimal? Width { get; set; }

    public decimal? Height { get; set; }

    public string? Status { get; set; }

    public bool? IsActive { get; set; }

    public string? CellCode { get; set; }

    public virtual Cell? CellCodeNavigation { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
