using System;
using System.Collections.Generic;

namespace ASMS.Repositories.Entities;

public partial class ContainerLocationLog
{
    public int ContainerLocationLogId { get; set; }

    public string? ContainerCode { get; set; }

    public string? OrderCode { get; set; }

    public string? Assign { get; set; }

    public DateOnly? UpdatedDate { get; set; }

    public string? OldFloor { get; set; }

    public string? CurrentFloor { get; set; }

    public virtual Container? ContainerCodeNavigation { get; set; }
}
