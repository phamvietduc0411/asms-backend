using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Model.Orders
{
    public class UpdateOrderProcessRequest
    {
        public string OrderCode { get; set; } = null!;
        public string EmployeeCode { get; set; } = null!;       
        public string? ActionByRole { get; set; }                
        public string NewStatus { get; set; } = null!;
        public string? NextAssign { get; set; }                   
        public string ActionType { get; set; } = null!;           
        public string? OrderDetailCode { get; set; }              
        public string? Image { get; set; }
    }
}
