using System;
using System.Collections.Generic;

namespace ASMS.Repositories.Entities;

public partial class Room
{
    public string RoomCode { get; set; } = null!;

    public string? FloorCode { get; set; }

    public int? RoomTypeId { get; set; }

    public decimal? Length { get; set; }

    public decimal? Width { get; set; }

    public decimal? Height { get; set; }

    public string? Status { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<Cell> Cells { get; set; } = new List<Cell>();

    public virtual Floor? FloorCodeNavigation { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual RoomType? RoomType { get; set; }
}
