using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Model.Storages
{
    public class StorageResponse
    {
        public string StorageCode { get; set; }
        public int? BuildingId { get; set; }
        public string? BuildingCode { get; set; }
        public int? StorageTypeId { get; set; }
        public string? StorageTypeName { get; set; }
        public int? ProductTypeId { get; set; }
        public string? ProductTypeName { get; set; }
        public decimal? Length { get; set; }
        public decimal? Width { get; set; }
        public decimal? Height { get; set; }
        public string? Status { get; set; }
        public bool? IsActive { get; set; }
    }
}
