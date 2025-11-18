using System;
using System.Collections.Generic;

namespace ASMS.Repositories.Entities;

public partial class OrderDetailService
{
    public int Id { get; set; }

    public int OrderDetailId { get; set; }

    public int ServiceId { get; set; }

    public virtual OrderDetail OrderDetail { get; set; } = null!;

    public virtual Service Service { get; set; } = null!;
}
