using System;
using System.Collections.Generic;

namespace ASMS.Repositories.Entities;

public partial class WorkflowTemplate
{
    public int WorkflowTemplateId { get; set; }

    public string? Name { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<WorkflowStep> WorkflowSteps { get; set; } = new List<WorkflowStep>();
}
