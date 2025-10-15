using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Model.TrackingHistories
{
    public class CreateTrackingHistoryRequest
    {
        [StringLength(50)]
        public string? OrderDetailCode { get; set; }

        [StringLength(50)]
        public string? OldStatus { get; set; }

        [StringLength(50)]
        public string? NewStatus { get; set; }

        [StringLength(20)]
        public string? ActionType { get; set; }

        public DateOnly? CreateAt { get; set; } 

        [StringLength(50)]
        public string? CurrentAssign { get; set; }

        [StringLength(50)]
        public string? NextAssign { get; set; }

        [StringLength(500)]
        public string? Image { get; set; }

        [Required]
        [StringLength(50)]
        public string OrderCode { get; set; }
    }
}
