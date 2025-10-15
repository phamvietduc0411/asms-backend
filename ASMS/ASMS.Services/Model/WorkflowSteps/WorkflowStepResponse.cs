using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Model.WorkflowSteps
{
    public class WorkflowStepResponse
    {
        public int WorkflowStepId { get; set; }
        public int? WorkflowTemplateId { get; set; }
        public string? WorkflowTemplateName { get; set; } 
        public string? Name { get; set; }
        public int? StepNumber { get; set; }
    }
}
