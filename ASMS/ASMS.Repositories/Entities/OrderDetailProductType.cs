using System;
using System.Collections.Generic;

namespace ASMS.Repositories.Entities;

public partial class OrderDetailProductType
{
    public int Id { get; set; }

    public int OrderDetailId { get; set; }

    public int ProductTypeId { get; set; }

    public bool? IsActive { get; set; }

    public virtual OrderDetail OrderDetail { get; set; } = null!;

    public virtual ProductType ProductType { get; set; } = null!;
}
