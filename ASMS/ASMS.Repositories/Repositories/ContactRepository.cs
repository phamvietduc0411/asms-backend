using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASMS.Repositories.Data;
using ASMS.Repositories.Entities;
using ASMS.Repositories.Infrastructures;
using ASMS.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ASMS.Repositories.Repositories
{
    public class ContactRepository : GenericRepository<Contact>, IContactRepository
    {
        private const string CONTACT_TYPE_REQUEST_TO_RETRIEVE = "request to retrieve";
        public ContactRepository(VstorageContext context, ILogger logger)
            : base(context, logger)
        {
        }

        public async Task<List<Contact>> GetWithFilterAsync(
            int pageNumber,
            int pageSize,
            string? customerCode,
            string? employeeCode,
            string? orderCode)
        {
            try
            {
                var query = _dbSet
                    .Include(c => c.CustomerCodeNavigation)
                    .Include(c => c.OrderCodeNavigation)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(customerCode))
                    query = query.Where(c => c.CustomerCode == customerCode);

                if (!string.IsNullOrWhiteSpace(employeeCode))
                    query = query.Where(c => c.EmployeeCode == employeeCode);

                if (!string.IsNullOrWhiteSpace(orderCode))
                    query = query.Where(c => c.OrderCode == orderCode);

                return await query
                    .OrderByDescending(c => c.ContactId)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting contacts with filter");
                throw;
            }
        }

        public async Task<int> GetTotalCountWithFilterAsync(
            string? customerCode,
            string? employeeCode,
            string? orderCode)
        {
            try
            {
                var query = _dbSet.AsQueryable();

                if (!string.IsNullOrWhiteSpace(customerCode))
                    query = query.Where(c => c.CustomerCode == customerCode);

                if (!string.IsNullOrWhiteSpace(employeeCode))
                    query = query.Where(c => c.EmployeeCode == employeeCode);

                if (!string.IsNullOrWhiteSpace(orderCode))
                    query = query.Where(c => c.OrderCode == orderCode);

                return await query.CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total count with filter");
                throw;
            }
        }
        public async Task<int> CountRequestToRetrieveByOrderCodeAsync(string orderCode)
        {
            try
            {
                return await _dbSet
                    .AsNoTracking()
                    .Where(c => c.OrderCode == orderCode &&
                               c.ContactType == CONTACT_TYPE_REQUEST_TO_RETRIEVE)
                    .CountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error counting request_to_retrieve contacts for order {Code}", orderCode);
                throw;
            }
        }
    }
}
