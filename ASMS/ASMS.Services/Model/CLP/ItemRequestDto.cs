using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Model.CLP
{
    public class ItemRequestDto
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; }
        public int ProductTypeID { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public decimal Weight { get; set; }
        public int Quantity { get; set; }
        public bool IsFragile { get; set; }
        public string? ImageUrl { get; set; }
        public string? Notes {  get; set; }

    }
}
