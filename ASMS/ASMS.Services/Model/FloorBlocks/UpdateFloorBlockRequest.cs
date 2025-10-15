using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Model.FloorBlocks
{
    public class UpdateFloorBlockRequest
    {
        public string FloorBlockCode { get; set; } = null!;
        public decimal? Length { get; set; }
        public decimal? Width { get; set; }
        public decimal? Height { get; set; }
        public string? Status { get; set; }
        public bool? IsActive { get; set; }
    }
}
