using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Model.Container
{
    public class ContainerResponse
    {
        public string ContainerCode { get; set; } = null!;
        public string? FloorCode { get; set; }
        public bool? IsActive { get; set; }
        public string? Status { get; set; }
        public decimal? Price { get; set; }
        public int? ProductTypeId { get; set; }
        public decimal? MaxWeight { get; set; }
        public decimal? CurrentWeight { get; set; }
        public decimal? PositionX { get; set; }
        public decimal? PositionY { get; set; }
        public decimal? PositionZ { get; set; }
        public DateTime? LastOptimizedDate { get; set; }
        public decimal? OptimizationScore { get; set; }
        public string? Notes { get; set; }
        public string? ImageUrl { get; set; }
        public string? Type { get; set; }
    }
}
