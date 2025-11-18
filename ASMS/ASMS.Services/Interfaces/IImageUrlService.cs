using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASMS.Services.Model.Image;

namespace ASMS.Services.Interfaces
{
    public interface IImageUrlService
    {
        Task<BatchUpdateImageUrlResponse> BatchUpdateImageUrlAsync(BatchUpdateImageUrlRequest request);
    }
}
