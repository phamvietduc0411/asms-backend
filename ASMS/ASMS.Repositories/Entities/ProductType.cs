using System;
using System.Collections.Generic;

namespace ASMS.Repositories.Entities;

public partial class ProductType
{
    public int ProductTypeId { get; set; }

    public string? Name { get; set; }

    public string? Status { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsFragile { get; set; }

    public bool? CanStack { get; set; }

    public string? Description { get; set; }

    public string? Vname { get; set; }

    public virtual ICollection<Container> Containers { get; set; } = new List<Container>();

    public virtual ICollection<OrderDetailProductType> OrderDetailProductTypes { get; set; } = new List<OrderDetailProductType>();

    public virtual ICollection<Storage> Storages { get; set; } = new List<Storage>();
}
