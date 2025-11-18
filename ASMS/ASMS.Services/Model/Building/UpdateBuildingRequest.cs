using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Model.Building
{
    public class UpdateBuildingRequest
    {
        public string? BuildingCode { get; set; }

        public string? Name { get; set; }

        public string? Area { get; set; }

        public string? Address { get; set; }

        public string? Status { get; set; }

        public bool? IsActive { get; set; }

        public int BuildingId { get; set; }

        public string? ImageUrl { get; set; }
    }
}
