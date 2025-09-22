using System;
using System.Collections.Generic;

namespace ASMS.Repositories.Entities;

public partial class WorkflowTemplate
{
    public int WorkflowTemplateId { get; set; }

    public string? TemplateName { get; set; }

    public int? RoomTypeId { get; set; }

    public string? Status { get; set; }

    public virtual RoomType? RoomType { get; set; }

    public virtual ICollection<WorkflowStep> WorkflowSteps { get; set; } = new List<WorkflowStep>();
}
