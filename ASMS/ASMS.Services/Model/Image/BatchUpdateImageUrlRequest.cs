using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Model.Image
{
    public class BatchUpdateImageUrlRequest
    {
        [Required(ErrorMessage = "TableName là bắt buộc")]
        [RegularExpression("^(Container|Storage|Shelf|Floor|ContainerType)$",
            ErrorMessage = "TableName phải là một trong: Container, Storage, Shelf, Floor")]
        public string TableName { get; set; } = null!;

        [Required]
        public List<ImageUrlUpdateItem> Items { get; set; } = new List<ImageUrlUpdateItem>();
    }
}
