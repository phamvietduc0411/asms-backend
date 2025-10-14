using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Model.StorageBlocks
{
    public class StorageBlockResponse
    {
        public string StorageBlockCode { get; set; }
        public string? StorageCode { get; set; }
        public decimal? Length { get; set; }
        public decimal? Width { get; set; }
        public decimal? Height { get; set; }
        public string? Status { get; set; }
        public bool? IsActive { get; set; }
    }
}
