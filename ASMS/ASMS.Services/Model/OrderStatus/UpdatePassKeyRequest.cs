using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Model.OrderStatus
{
    public class UpdatePassKeyRequest
    {
        public string OrderCode { get; set; } = null!;
        public string OldPassKey { get; set; } 
        public string NewPassKey { get; set; }
    }
}
