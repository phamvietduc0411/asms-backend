using ASMS.Repositories.Common;
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
    public class ProductTypeRepository : GenericRepository<ProductType> , IProductTypeRepository
    {
        public ProductTypeRepository(
       VstorageContext context, ILogger logger) : base(context, logger)
        {
        }
        public async Task<PaginatedList<ProductType>> GetWithFilterAsync(bool? isActive, int pageNumber, int pageSize)
        {
            var query = _context.ProductTypes.AsQueryable();

            if (isActive.HasValue)
            {
                query = query.Where(pt => pt.IsActive == isActive.Value);
            }

            query = query.OrderBy(pt => pt.ProductTypeId);

            return await PaginatedList<ProductType>.CreateAsync(query, pageNumber, pageSize);
        }
        public async Task<List<ProductType>> GetByIdsAsync(List<int> ids)
        {
            return await _context.ProductTypes
                .Where(pt => ids.Contains(pt.ProductTypeId) && pt.IsActive == true)
                .ToListAsync();
        }

    }
}
