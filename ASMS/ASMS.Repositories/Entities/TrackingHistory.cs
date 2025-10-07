using System;
using System.Collections.Generic;

namespace ASMS.Repositories.Entities;

public partial class TrackingHistory
{
    public int TrackingHistoryId { get; set; }

    public string? OrderDetailCode { get; set; }

    public string? OldStatus { get; set; }

    public string? NewStatus { get; set; }

    public string? ActionType { get; set; }

    public DateOnly? CreateAt { get; set; }

    public string? CurrentAssign { get; set; }

    public string? NextAssign { get; set; }

    public string? Image { get; set; }

    public string? OrderCode { get; set; }

    public virtual Order? OrderCodeNavigation { get; set; }
}
