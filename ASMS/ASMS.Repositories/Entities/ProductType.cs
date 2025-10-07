using System;
using System.Collections.Generic;

namespace ASMS.Repositories.Entities;

public partial class ProductType
{
    public int ProductTypeId { get; set; }

    public string? Name { get; set; }

    public string? Status { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<ContainerType> ContainerTypes { get; set; } = new List<ContainerType>();

    public virtual ICollection<Storage> Storages { get; set; } = new List<Storage>();
}
