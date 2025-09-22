using System;
using System.Collections.Generic;

namespace ASMS.Repositories.Entities;

public partial class WorkflowStep
{
    public int WorkflowStepId { get; set; }

    public int? WorkflowTemplateId { get; set; }

    public int? StepNumber { get; set; }

    public virtual WorkflowTemplate? WorkflowTemplate { get; set; }
}
