using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Model.Container
{
    public class UpdateContainerPositionRequest
    {
        public List<ContainerPositionItem> Containers { get; set; } = new List<ContainerPositionItem>();
    }
}
