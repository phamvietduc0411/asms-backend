using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Model.StorageTypes
{
    public class CreateStorageTypeRequest
    {
        [Required]
        public int StorageTypeId { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public decimal? Length { get; set; }

        public decimal? Width { get; set; }

        public decimal? Height { get; set; }

        public decimal? TotalVolume { get; set; }

        public decimal? Area { get; set; }

        public decimal? Price { get; set; }

        public string? ImageUrl { get; set; }
    }
}
