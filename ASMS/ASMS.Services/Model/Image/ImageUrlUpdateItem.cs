using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Model.Image
{
    public class ImageUrlUpdateItem
    {
        [Required]
        public string Code { get; set; } = null!;

        [Required]
        public string ImageUrl { get; set; } = null!;
    }
}
