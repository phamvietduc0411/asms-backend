using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Model.WorkflowTemplates
{
    public class CreateWorkflowTemplateRequest
    {
        [Required]
        public int WorkflowTemplateId { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        public int? StorageTypeId { get; set; }

        [StringLength(10)]
        public string? Status { get; set; }
    }
}
