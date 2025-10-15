using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASMS.Repositories.Interfaces;

namespace ASMS.Repositories.Infrastructures
{
    public interface IUnitOfWork
    {
        IEmployeeRoleRepository EmployeeRoles { get; }
        IWorkflowTemplateRepository WorkflowTemplates { get; }
        IWorkflowStepRepository WorkflowSteps { get; }
        IServiceRepository Services { get; } 
        ITrackingHistoryRepository TrackingHistories { get; }
        IStorageBlockRepository StorageBlocks { get; }
        IBuildingRepository Building { get; }
        IFloorRepository Floors { get; }

        Task CompleteAsync();
    }
}
