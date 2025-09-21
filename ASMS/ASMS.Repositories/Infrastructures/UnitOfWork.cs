using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Repositories.Infrastructures
{
    public class UnitOfWork : IUnitOfWork
    {
        public Task CompleteAsync()
        {
            throw new NotImplementedException();
        }
    }
}
