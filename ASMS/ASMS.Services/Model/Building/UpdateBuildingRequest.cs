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

        public int? FloorQuantity { get; set; }

        public string? Status { get; set; }

        public bool? IsActive { get; set; }
    }
}
