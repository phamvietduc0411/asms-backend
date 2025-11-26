using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Model.Floor
{
    public class FloorResponse
    {
        public string FloorCode { get; set; } = null!;
        public string? ShelfCode { get; set; }
        public int? FloorNumber { get; set; }
        public string? Status { get; set; }
        public bool? IsActive { get; set; }
        public decimal? Length { get; set; }
        public decimal? Width { get; set; }
        public decimal? Height { get; set; }
        public decimal? MaxWeight { get; set; }
        public decimal? CurrentWeight { get; set; }
        public int? MaxContainers { get; set; }
        public int? CurrentContainerCount { get; set; }
        public decimal? UtilizationRate { get; set; }
        public string? ImageUrl { get; set; }
    }
}
