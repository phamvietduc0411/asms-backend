using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Model.FloorBlocks
{
    public class CreateFloorBlockRequest
    {
        public string FloorBlockCode { get; set; } = null!;
        public string? FloorCode { get; set; }
        public decimal? Length { get; set; }
        public decimal? Width { get; set; }
        public decimal? Height { get; set; }
        public string? Status { get; set; }
    }
}
