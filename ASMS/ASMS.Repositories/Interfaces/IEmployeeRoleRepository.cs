using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASMS.Repositories.Common;
using ASMS.Repositories.Entities;
using ASMS.Repositories.Infrastructures;

namespace ASMS.Repositories.Interfaces
{
    public interface IEmployeeRoleRepository :IGenericRepository<EmployeeRole>
    {
        Task<PaginatedList<EmployeeRole>> GetAllAsync(int pageNumber, int pageSize);
    }
}
