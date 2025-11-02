using System;
using System.Collections.Generic;

namespace ASMS.Repositories.Entities;

public partial class Floor
{
    public string FloorCode { get; set; } = null!;

    public string? ShelfCode { get; set; }

    public int? FloorNumber { get; set; }

    public string? Status { get; set; }

    public bool? IsActive { get; set; }

    public decimal? Length { get; set; }

    public decimal? Width { get; set; }

    public decimal? Height { get; set; }

    public decimal? PositionX { get; set; }

    public decimal? PositionY { get; set; }

    public decimal? PositionZ { get; set; }

    public decimal? MaxWeight { get; set; }

    public decimal? CurrentWeight { get; set; }

    public int? MaxContainers { get; set; }

    public int? CurrentContainerCount { get; set; }

    public decimal? UtilizationRate { get; set; }

    public virtual ICollection<Container> Containers { get; set; } = new List<Container>();

    public virtual ICollection<FloorBlock> FloorBlocks { get; set; } = new List<FloorBlock>();

    public virtual Shelf? ShelfCodeNavigation { get; set; }
}
