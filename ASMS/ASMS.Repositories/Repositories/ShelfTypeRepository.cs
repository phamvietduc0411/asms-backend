using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASMS.Repositories.Common;
using ASMS.Repositories.Data;
using ASMS.Repositories.Entities;
using ASMS.Repositories.Infrastructures;
using ASMS.Repositories.Interfaces;
using Microsoft.Extensions.Logging;

namespace ASMS.Repositories.Repositories
{
    public class ShelfTypeRepository : GenericRepository<ShelfType>, IShelfTypeRepository
    {
    
        public ShelfTypeRepository(VstorageContext context, ILogger logger) : base(context, logger)
        {
        }

        public async Task<PaginatedList<ShelfType>> GetAllAsync(int pageNumber, int pageSize)
        {
            var query = _context.ShelfTypes.OrderBy(st => st.ShelfTypeId);
            return await PaginatedList<ShelfType>.CreateAsync(query, pageNumber, pageSize);
        }

        public async Task<ShelfType> GetByIdAsync(int id)
        {
            return await _context.ShelfTypes.FindAsync(id);
        }

        public async Task AddAsync(ShelfType shelfType)
        {
            await _context.ShelfTypes.AddAsync(shelfType);
        }

        public async Task UpdateAsync(ShelfType shelfType)
        {
            _context.ShelfTypes.Update(shelfType);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(ShelfType shelfType)
        {
            _context.ShelfTypes.Remove(shelfType);
            await Task.CompletedTask;
        }
    }
}
