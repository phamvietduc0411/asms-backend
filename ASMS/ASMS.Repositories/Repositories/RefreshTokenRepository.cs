using ASMS.Repositories.Data;
using ASMS.Repositories.Entities;
using ASMS.Repositories.Infrastructures;
using ASMS.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Repositories.Repositories
{
    public class RefreshTokenRepository : GenericRepository<RefreshToken>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(
        VstorageContext context, ILogger logger) : base(context, logger)
        {
        }
        public async Task<RefreshToken?> GetByTokenAsync(string token)
        {
            return await _context.RefreshToken.FirstOrDefaultAsync(t => t.Token == token);
        }

    }
}
