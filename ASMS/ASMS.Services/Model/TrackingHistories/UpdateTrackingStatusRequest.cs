using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Model.TrackingHistories
{
    public class UpdateTrackingStatusRequest
    {
        public string? OrderDetailCode { get; set; }

        [Required(ErrorMessage = "OrderCode is required")]
        public string OrderCode { get; set; } = null!;

        [Required(ErrorMessage = "OldStatus is required")]
        public string OldStatus { get; set; } = null!;

        [Required(ErrorMessage = "NewStatus is required")]
        public string NewStatus { get; set; } = null!;

        [Required(ErrorMessage = "ActionType is required")]
        public string ActionType { get; set; } = null!;

        [Required(ErrorMessage = "CurrentAssign (current handler) is required")]
        public string CurrentAssign { get; set; } = null!;

        
        public string? NextAssign { get; set; }

        public string? Image { get; set; }
    }
}


