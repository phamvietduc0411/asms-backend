using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Model.Image
{
    public class BatchUpdateImageUrlResponse
    {
        public string TableName { get; set; } = null!;
        public int TotalItems { get; set; }
        public int SuccessCount { get; set; }
        public int FailedCount { get; set; }
        public List<string> SuccessCodes { get; set; } = new List<string>();
        public List<UpdateErrorDetail> Errors { get; set; } = new List<UpdateErrorDetail>();
        public string Message { get; set; } = null!;
    }
}
