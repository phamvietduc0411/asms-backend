using System;
using System.Collections.Generic;

namespace ASMS.Repositories.Entities;

public partial class Cell
{
    public string CellCode { get; set; } = null!;

    public string? RoomCode { get; set; }

    public string? Status { get; set; }

    public bool? IsActive { get; set; }

    public decimal? Price { get; set; }

    public decimal? Length { get; set; }

    public decimal? Width { get; set; }

    public decimal? Height { get; set; }

    public virtual ICollection<Box> Boxes { get; set; } = new List<Box>();

    public virtual Room? RoomCodeNavigation { get; set; }
}
