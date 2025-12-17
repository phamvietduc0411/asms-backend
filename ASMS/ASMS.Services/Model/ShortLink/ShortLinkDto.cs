using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Model.ShortLink
{
    public class ShortLinkDto
    {
        public string ShortUrl { get; set; } = null!;
        public string ShortCode { get; set; } = null!;
        public string OriginalUrl { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }
}
