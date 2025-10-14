using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Model.StorageBlocks
{
    public class UpdateStorageBlockRequest
    {
        [StringLength(50)]
        public string? StorageCode { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Length must be greater than or equal to 0")]
        public decimal? Length { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Width must be greater than or equal to 0")]
        public decimal? Width { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Height must be greater than or equal to 0")]
        public decimal? Height { get; set; }

        [StringLength(10)]
        public string? Status { get; set; }

        public bool? IsActive { get; set; }
    }
}
