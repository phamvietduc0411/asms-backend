using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Model.ShortLink
{
    public class CreateShortLinkRequest
    {
        public string OriginalUrl { get; set; } = null!;
        public int? ExpiresInDays { get; set; }

        public string? OrderCode { get; set; }
        public int? OrderDetailId { get; set; }
    }
}
