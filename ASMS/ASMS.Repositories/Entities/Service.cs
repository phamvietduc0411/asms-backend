using System;
using System.Collections.Generic;

namespace ASMS.Repositories.Entities;

public partial class Service
{
    public int ServiceId { get; set; }

    public string? Name { get; set; }

    public decimal? Price { get; set; }

    public virtual ICollection<OrderDetailService> OrderDetailServices { get; set; } = new List<OrderDetailService>();
}
