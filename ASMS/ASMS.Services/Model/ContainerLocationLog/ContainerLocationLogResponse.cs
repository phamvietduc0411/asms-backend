using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Model.ContainerLocationLog
{
    public class ContainerLocationLogResponse
    {
        public int ContainerLocationLogId { get; set; }
        public string? ContainerCode { get; set; }
        public string? OrderCode { get; set; }
        public string? PerformedBy { get; set; }
        public DateOnly? UpdatedDate { get; set; }
        public string? OldFloor { get; set; }
        public string? CurrentFloor { get; set; }
    }
}
