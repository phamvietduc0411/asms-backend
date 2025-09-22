using System;
using System.Collections.Generic;

namespace ASMS.Repositories.Entities;

public partial class OrderDetail
{
    public int OrderDetailId { get; set; }

    public string? OrderCode { get; set; }

    public string? RoomCode { get; set; }

    public string? BoxCode { get; set; }

    public int? ServiceId { get; set; }

    public string? Quantity { get; set; }

    public decimal? SubTotal { get; set; }

    public string? Address { get; set; }

    public string? Image { get; set; }

    public virtual Box? BoxCodeNavigation { get; set; }

    public virtual Order? OrderCodeNavigation { get; set; }

    public virtual Room? RoomCodeNavigation { get; set; }

    public virtual Service? Service { get; set; }
}
