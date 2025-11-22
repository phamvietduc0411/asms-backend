using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Model.Container
{
    public class ContainerPositionItem
    {
        [Required(ErrorMessage = "ContainerCode là bắt buộc")]
        public string ContainerCode { get; set; } = null!;

        public decimal PositionX { get; set; }

        public decimal PositionY { get; set; }

        public decimal PositionZ { get; set; }
    }
}
