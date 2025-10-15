using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Model.ContainerLocationLog
{
    public class UpdateContainerLocationLogRequest
    {
        public string? Assign { get; set; }
        public DateOnly? UpdatedDate { get; set; }
        public string? OldFloor { get; set; }
        public string? CurrentFloor { get; set; }
    }
}
