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

    public int? MaxStackLayers { get; set; }

    public decimal? AverageWeight { get; set; }

    public string? ShapeType { get; set; }

    public int? PlacementPriority { get; set; }

    public string? PreferredZone { get; set; }

    public bool? RequireMoistureControl { get; set; }

    public bool? RequireVentilation { get; set; }

    public bool? AvoidSunlight { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<Container> Containers { get; set; } = new List<Container>();

    public virtual ICollection<Item> Items { get; set; } = new List<Item>();

    public virtual ICollection<Storage> Storages { get; set; } = new List<Storage>();
}
