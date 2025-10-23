using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Model.StorageTypes
{
    public class UpdateStorageTypeRequest
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
    }
}
