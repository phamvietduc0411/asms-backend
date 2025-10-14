using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Model.WorkflowSteps
{
    public class UpdateWorkflowStepRequest
    {
        [Required]
        public int WorkflowTemplateId { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        public int StepNumber { get; set; }
    }
}
