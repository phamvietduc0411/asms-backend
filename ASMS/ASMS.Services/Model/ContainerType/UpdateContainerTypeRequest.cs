using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Model.ContainerType
{
    public class UpdateContainerTypeRequest
    {
        public decimal? Volume { get; set; }

        public int? ProductTypeId { get; set; }

        public string? Name { get; set; }

        public string? Status { get; set; }

        public bool? IsActive { get; set; }

        public decimal? Price { get; set; }

    }
}
