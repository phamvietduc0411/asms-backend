using ASMS.Repositories.Common;
using ASMS.Repositories.Data;
using ASMS.Repositories.Entities;
using ASMS.Repositories.Infrastructures;
using ASMS.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ASMS.Repositories.Repositories
{
    public class EmployeeRepository : GenericRepository<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(VstorageContext context, ILogger logger) : base(context, logger)
        {
        }
        public async Task<Employee?> GetEmployeeByEmailAsync(string email)
        {
            return await _dbSet
                .Include(e => e.EmployeeRole)
                .FirstOrDefaultAsync(e => e.Username == email);
        }

        public async Task<Employee?> GetByCodeAsync(string employeeCode)
        {
            return await _dbSet
                .Include(e => e.EmployeeRole)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.EmployeeCode == employeeCode);
        }


        public async Task<PaginatedList<Employee>> GetWithFilterAsync(string? roleName,string? status,int pageNumber, int pageSize)
        {
            var query = _context.Employees
                .Include(e => e.EmployeeRole)
                .AsQueryable();

            if (!string.IsNullOrEmpty(roleName))
            {
                query = query.Where(e => e.EmployeeRole != null && e.EmployeeRole.Name == roleName);
            }
            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(e => e.Status != null && e.Status == status);
            }

            query = query.OrderBy(e => e.EmployeeCode);

            return await PaginatedList<Employee>.CreateAsync(query, pageNumber, pageSize);
        }
        public virtual async Task<Employee?> GetEntityByIdAsync(int id)
        {
            return await _dbSet
                .Include(e => e.EmployeeRole)
                .FirstOrDefaultAsync(e => e.Id == id);
        }
        public async Task<IEnumerable<Employee>> GetByRoleAsync(string roleName)
        {
            return await _dbSet
                .Include(e => e.EmployeeRole)
                .Where(e => e.EmployeeRole != null && e.EmployeeRole.Name == roleName)
                .ToListAsync();
        }

        public async Task<Employee?> GetAvailableEmployeeByRoleAsync(string roleName)
        {
            return await _dbSet
                .Include(e => e.EmployeeRole)
                .Where(e => e.EmployeeRole != null
                    && e.EmployeeRole.Name == roleName
                    && e.Status == "Active"
                    && e.IsActive == true)
                .OrderBy(e => e.OrderActionCount ?? 0)
                .FirstOrDefaultAsync();
        }
        public async Task<IEnumerable<Employee>> GetAllAsync()
        {
            return await _dbSet
                .Include(e => e.EmployeeRole)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Employee?> GetAvailableDeliveryForOrder()
            => await _context.Employees.FirstOrDefaultAsync(e => e.Status == "Available");

       //======= ==== Deliverry ===============
        //        1. Offline / Inactive

        //Không đăng nhập hoặc không bật trạng thái nhận đơn

        //2. Available

        //Đang mở app và sẵn sàng nhận đơn

        //3. Accepting

        //Đang chờ xác nhận nhận đơn(nếu có logic countdown)

        //4. Assigned

        //Hệ thống đã giao một order cho deliver

        //5. PickingUp

        //Đang đến nơi lấy hàng

        //6. OnTheWay / Delivering

        //Đang giao đến khách hàng

        //7. Completed

        //Giao xong đơn hàng

        //8. Cancelled / Returned

        //Hoàn tác / giao không thành công


        //====================== Ware house staff =====
        //        1. Offline / Inactive

        //Không làm việc, không đăng nhập.

        //2. Available

        //Đang sẵn sàng xử lý đơn trong kho.

        //3. Processing

        //Đang xử lý một đơn (soạn hàng, đóng gói, chuẩn bị giao).

        //4. Completed

        //Xử lý xong một nhiệm vụ.

        //5. Paused(tùy chọn)
    }
}
