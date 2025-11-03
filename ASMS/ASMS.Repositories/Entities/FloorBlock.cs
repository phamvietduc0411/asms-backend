using System;
using System.Collections.Generic;

namespace ASMS.Repositories.Entities;

public partial class FloorBlock
{
    public string FloorBlockCode { get; set; } = null!;

    public string? FloorCode { get; set; }

    public decimal? Length { get; set; }

    public decimal? Width { get; set; }

    public decimal? Height { get; set; }

    public string? Status { get; set; }

    public bool? IsActive { get; set; }

    public decimal? PositionX { get; set; }

    public decimal? PositionY { get; set; }

    public decimal? PositionZ { get; set; }

    public virtual Floor? FloorCodeNavigation { get; set; }
}
