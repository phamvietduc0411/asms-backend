using System;
using System.Collections.Generic;

namespace ASMS.Repositories.Entities;

public partial class ShelfType
{
    public int ShelfTypeId { get; set; }

    public string Name { get; set; } = null!;

    public decimal Length { get; set; }

    public decimal Width { get; set; }

    public decimal Height { get; set; }

    public decimal? Price { get; set; }

    public string? ImageUrl { get; set; }

    public virtual ICollection<Shelf> Shelves { get; set; } = new List<Shelf>();
}
