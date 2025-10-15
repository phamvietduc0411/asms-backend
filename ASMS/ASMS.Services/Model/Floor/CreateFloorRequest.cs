using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Model.Floor
{
    public class CreateFloorRequest
    {
        public string FloorCode { get; set; } = null!;
        public string? ShelfCode { get; set; }
        public int? FloorNumber { get; set; }
        public string? Status { get; set; }
        public bool? IsActive { get; set; }
        public decimal? Length { get; set; }
        public decimal? Width { get; set; }
        public decimal? Height { get; set; }
    }
}
