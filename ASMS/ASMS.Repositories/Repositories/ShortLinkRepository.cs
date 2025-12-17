using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASMS.Repositories.Data;
using ASMS.Repositories.Entities;
using ASMS.Repositories.Infrastructures;
using ASMS.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ASMS.Repositories.Repositories
{
    public class ShortLinkRepository : GenericRepository<ShortLink>, IShortLinkRepository
    {
        public ShortLinkRepository(VstorageContext context, ILogger logger) : base(context, logger)
        {
        }

        public async Task<ShortLink?> GetByShortCodeAsync(string shortCode)
        {
            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.ShortCode == shortCode);
        }

        public async Task<ShortLink?> GetByOriginalUrlAsync(string originalUrl)
        {
            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.OriginalUrl == originalUrl);
        }

        public async Task<bool> ShortCodeExistsAsync(string shortCode)
        {
            return await _dbSet
                .AnyAsync(x => x.ShortCode == shortCode);
        }

        public async Task IncrementClickCountAsync(string shortCode)
        {
            var shortLink = await _dbSet
                .FirstOrDefaultAsync(x => x.ShortCode == shortCode);

            if (shortLink != null)
            {
                shortLink.ClickCount++;
            }
        }

        public async Task<List<ShortLink>> GetExpiredLinksAsync()
        {
            return await _dbSet
                .Where(x => x.ExpiresAt != null && x.ExpiresAt < DateTime.UtcNow)
                .ToListAsync();
        }
        public async Task DeleteAsync(ShortLink entity)
        {
            _dbSet.Remove(entity);
            await Task.CompletedTask;
        }
    }
}
