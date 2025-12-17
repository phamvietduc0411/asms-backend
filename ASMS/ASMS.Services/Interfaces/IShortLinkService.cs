using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASMS.Repositories.Entities;

namespace ASMS.Services.Interfaces
{
    public interface IShortLinkService
    {
        Task<string> GenerateShortCodeAsync(string originalUrl, string? orderCode = null, int? orderDetailId = null, int? expiresInDays = null);
        Task<ShortLink?> GetByShortCodeAsync(string shortCode);
        Task<ShortLink?> GetByOriginalUrlAsync(string originalUrl);
        Task IncrementClickCountAsync(string shortCode);
        Task DeleteExpiredLinksAsync();
        Task<ShortLink?> GetByOrderCodeAsync(string orderCode); 
        Task<ShortLink?> GetByOrderDetailIdAsync(int orderDetailId);
    }
}
