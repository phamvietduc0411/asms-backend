using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Model.Container
{
    public class UpdateContainerPositionRequest
    {
        public string ContainerCode { get; set; }
        public decimal PositionX { get; set; }
        public decimal PositionY { get; set; }
        public decimal PositionZ { get; set; }
    }
}
