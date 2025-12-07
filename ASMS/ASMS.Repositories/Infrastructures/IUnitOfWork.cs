using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASMS.Repositories.Data;
using ASMS.Repositories.Interfaces;

namespace ASMS.Repositories.Infrastructures
{
    public interface IUnitOfWork
    {
        VstorageContext Context { get; }
        IEmployeeRoleRepository EmployeeRoles { get; }
        IWorkflowTemplateRepository WorkflowTemplates { get; }
        IWorkflowStepRepository WorkflowSteps { get; }
        IServiceRepository Services { get; } 
        ITrackingHistoryRepository TrackingHistories { get; }
        IBuildingRepository Building { get; }
        IProductTypeRepository ProductType { get; }
        IFloorRepository Floors { get; }
        IContainerTypeRepository ContainerType { get; }
        IShelfRepository Shelves { get; }
        IContainerLocationLogRepository ContainerLocationLogs { get; }
        IOrderDetailRepository OrderDetails { get; }
        IContainerRepository Containers { get; }
        ICustomerRepository Customer { get; }
        IEmployeeRepository Employee { get; }
        IStorageRepository Storages { get; }
        IOrderRepository Orders { get; }
        IStorageTypeRepository StorageTypes { get; }
        IPaymentHistoryRepository PaymentHistories { get; }
        IRefreshTokenRepository RefreshToken { get; }
        IShelfTypeRepository ShelvesTypes { get; }
        IOrderDetailProductTypeRepository OrderDetailProductTypes { get; }
        IOrderDetailServiceRepository OrderDetailServices { get; }
        IPaymentResultRepository PaymentResults { get; }
        IPricingRepository Pricings { get; }
        IShippingRateRepository ShippingRates { get; }
        IContactRepository Contacts { get; }
        IBusinessRuleRepository BusinessRules { get; }
        Task CompleteAsync();
    }
}
