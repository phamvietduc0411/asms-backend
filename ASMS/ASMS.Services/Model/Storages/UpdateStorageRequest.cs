using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Model.Storages
{
    public class UpdateStorageRequest
    {
        public int? BuildingId { get; set; }
        public int? StorageTypeId { get; set; }
        public int? ProductTypeId { get; set; }
        public decimal? Length { get; set; }
        public decimal? Width { get; set; }
        public decimal? Height { get; set; }

        [StringLength(10)]
        public string? Status { get; set; }

        public bool? IsActive { get; set; }
    }
}
