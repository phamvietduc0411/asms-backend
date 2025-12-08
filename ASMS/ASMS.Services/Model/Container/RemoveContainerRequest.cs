using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Model.Container
{
    public class RemoveContainerRequest
    {
        public string ContainerCode { get; set; }
        public string? OrderCode { get; set; }  
        public string? PerformedBy { get; set; }
    }
}
