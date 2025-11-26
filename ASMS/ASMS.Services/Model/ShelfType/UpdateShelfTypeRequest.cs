using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Model.ShelfType
{
    public class UpdateShelfTypeRequest
    {
        public string Name { get; set; } = null!;
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public decimal? Price { get; set; }
        public string? ImageUrl { get; set; }
    }
}
