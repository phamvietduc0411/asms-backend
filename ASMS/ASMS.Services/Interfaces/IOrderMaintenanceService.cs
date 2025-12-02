using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASMS.Services.Interfaces
{
    public interface IOrderMaintenanceService
    {
        Task CheckAndProcessOverdueOrdersAsync();
        Task MoveOldOverdueOrdersToExpiredStorageAsync();
    }
}
