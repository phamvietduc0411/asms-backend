using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

    }
}

