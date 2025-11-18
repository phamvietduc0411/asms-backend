using System;
using System.Collections.Generic;

namespace ASMS.Repositories.Entities;

public partial class ContainerType
{
    public int ContainerTypeId { get; set; }

    public string Type { get; set; } = null!;

    public decimal? Length { get; set; }

    public decimal? Width { get; set; }

    public decimal? Height { get; set; }

    public string? ImageUrl { get; set; }

    public decimal? Price { get; set; }

    public virtual ICollection<Container> Containers { get; set; } = new List<Container>();
}
