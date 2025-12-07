using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASMS.Repositories.Entities;
using ASMS.Repositories.Infrastructures;

namespace ASMS.Repositories.Interfaces
{
    public interface IContactRepository : IGenericRepository<Contact>
    {
        Task<List<Contact>> GetWithFilterAsync(
            int pageNumber,
            int pageSize,
            string? customerCode,
            string? employeeCode,
            string? orderCode);

        Task<int> GetTotalCountWithFilterAsync(
            string? customerCode,
            string? employeeCode,
            string? orderCode);
    }
}
