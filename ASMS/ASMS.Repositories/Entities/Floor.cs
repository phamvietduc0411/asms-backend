using System;
using System.Collections.Generic;

namespace ASMS.Repositories.Entities;

public partial class Floor
{
    public string FloorCode { get; set; } = null!;

    public string? BuildingCode { get; set; }

    public int? FloorNumber { get; set; }

    public string? Status { get; set; }

    public bool? IsActive { get; set; }

    public virtual Building? BuildingCodeNavigation { get; set; }

    public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();
}
