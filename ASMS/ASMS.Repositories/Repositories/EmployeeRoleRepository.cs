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
    public class EmployeeRoleRepository : GenericRepository<EmployeeRole>, IEmployeeRoleRepository
    {
        public EmployeeRoleRepository(
            VstorageContext context, ILogger logger) : base(context,logger)
        {
        }
        public async Task<PaginatedList<EmployeeRole>> GetAllAsync(int pageNumber, int pageSize)
        {
            var query = _context.EmployeeRoles.OrderBy(er => er.EmployeeRoleId);
            return await PaginatedList<EmployeeRole>.CreateAsync(query, pageNumber, pageSize);
        }
    }
}

