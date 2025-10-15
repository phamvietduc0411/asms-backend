using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Model.WorkflowTemplates
{
    public class WorkflowTemplateResponse
    {
        public int WorkflowTemplateId { get; set; }
        public string? Name { get; set; }
        public int? StorageTypeId { get; set; }
        public string? StorageTypeName { get; set; }
        public string? Status { get; set; }
    }
}