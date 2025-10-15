using ASMS.Repositories.Data;
using ASMS.Repositories.Entities;
using ASMS.Repositories.Infrastructures;
using ASMS.Repositories.Interfaces;
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

    }
}
