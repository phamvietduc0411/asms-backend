using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Model.ShortLink
{
    public class FindShortLinkRequest
    {
        public string OriginalUrl { get; set; } = null!;
    }
}
