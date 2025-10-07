using System;
using System.Collections.Generic;

namespace ASMS.Repositories.Entities;

public partial class ContainerType
{
    public int ContainerTypeId { get; set; }

    public decimal? Volume { get; set; }

    public int? ProductTypeId { get; set; }

    public string? Name { get; set; }

    public string? Status { get; set; }

    public bool? IsActive { get; set; }

    public decimal? Price { get; set; }

    public virtual ProductType? ProductType { get; set; }
}
