using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Model.OrderStatus
{
    public class ResetPassKeyResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string? OrderCode { get; set; }
    }
}
