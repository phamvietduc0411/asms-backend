using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Model.ContainerType
{
    public class GetContainerTypeResponse
    {
        public int ContainerTypeId { get; set; }
        public string Type { get; set; } = null!;
        public decimal? Length { get; set; }
        public decimal? Width { get; set; }
        public decimal? Height { get; set; }
        public string? ImageUrl { get; set; }
        public decimal? Price { get; set; }
        public int? AvailableQuantityInAc { get; set; }

        public int? AvailableQuantityInNor { get; set; }
    }
}
