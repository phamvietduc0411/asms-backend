using System;
using System.Collections.Generic;

namespace ASMS.Repositories.Entities;

public partial class Service
{
    public int ServiceId { get; set; }

    public string? Type { get; set; }

    public decimal? Price { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
