using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Model.Shelves
{
    public class ShelfResponse
    {
        public string ShelfCode { get; set; } = null!;
        public string? StorageCode { get; set; }
        public string? Status { get; set; }
        public bool? IsActive { get; set; }
        public decimal? Length { get; set; }
        public decimal? Width { get; set; }
        public decimal? Height { get; set; }
    }
}
