using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASMS.Repositories.Data;
using ASMS.Repositories.Interfaces;
using ASMS.Repositories.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ASMS.Repositories.Infrastructures
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly VstorageContext _context;

        private readonly ILogger _logger;

        public IEmployeeRoleRepository EmployeeRoles { get; private set; }

        public UnitOfWork(
            VstorageContext context,
            ILoggerFactory loggerFactory)
        {
            _context = context;

            _logger = loggerFactory.CreateLogger("logs");
            EmployeeRoles = new EmployeeRoleRepository(_context, _logger);
        }
        public async Task CompleteAsync() => await _context.SaveChangesAsync();
    }
}
