using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Model.CLP
{
    public class ContainerSuggestionDto
    {
        public string ContainerCode { get; set; } = null!;
        public decimal Score { get; set; }

        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }

        public string FloorCode { get; set; } = null!;
        public int FloorNumber { get; set; }
        public string ShelfCode { get; set; } = null!;
        public string StorageCode { get; set; } = null!;
        public string BuildingCode { get; set; } = null!;
        public string BuildingName { get; set; } = null!;
    }
}
