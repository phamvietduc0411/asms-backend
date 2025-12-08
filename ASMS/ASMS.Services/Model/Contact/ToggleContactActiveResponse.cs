using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Model.Contact
{
    public class ToggleContactActiveResponse
    {
        public int ContactId { get; set; }
        public bool IsActive { get; set; }
        public string Message { get; set; }
    }
}
