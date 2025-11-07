using ASMS.Repositories.Data;
using ASMS.Repositories.Entities;
using ASMS.Repositories.Interfaces;
using ASMS.Repositories.Repositories;
using Microsoft.Extensions.Logging;

namespace ASMS.Repositories.Infrastructures
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly VstorageContext _context;
        private readonly ILogger _logger;
        public IEmployeeRoleRepository EmployeeRoles { get; private set; }
        public IWorkflowTemplateRepository WorkflowTemplates { get; private set; }
        public IWorkflowStepRepository WorkflowSteps { get; private set; }
        public IServiceRepository Services { get; private set; }
        public ITrackingHistoryRepository TrackingHistories { get; private set; }
        public IBuildingRepository Building { get; private set; }
        public IFloorRepository Floors { get; private set; }
        public IProductTypeRepository ProductType { get; private set; }
        public IContainerTypeRepository ContainerType { get; private set; }
        public IShelfRepository Shelves { get; private set; }
        public IContainerLocationLogRepository ContainerLocationLogs { get; private set; }
        public IOrderDetailRepository OrderDetails { get; private set; }
        public IContainerRepository Containers { get; private set; }
        public ICustomerRepository Customer { get; private set; }
        public IEmployeeRepository Employee { get; private set; }
        public IStorageRepository Storages { get; private set; }
        public IOrderRepository Orders { get; private set; }
        public IStorageTypeRepository StorageTypes { get; private set; }
        public IPaymentHistoryRepository PaymentHistories { get; private set; }






        public UnitOfWork(
            VstorageContext context,
            ILoggerFactory loggerFactory)
        {
            _context = context;
            _logger = loggerFactory.CreateLogger("logs");
            EmployeeRoles = new EmployeeRoleRepository(_context, _logger);
            WorkflowTemplates = new WorkflowTemplateRepository(_context, _logger);
            WorkflowSteps = new WorkflowStepRepository(_context, _logger);
            Services = new ServiceRepository(_context, _logger);
            TrackingHistories = new TrackingHistoryRepository(_context, _logger);   
            Building = new BuildingRepository(_context, _logger);
            ProductType = new ProductTypeRepository(_context, _logger);
            Floors = new FloorRepository(_context, _logger);
            ContainerType = new ContainerTypeRepository(_context, _logger);
            Shelves = new ShelfRepository(_context, _logger);
            Containers = new ContainerRepository(_context, _logger);
            Customer = new CustomerRepository(_context, _logger);
            Employee = new EmployeeRepository(_context, _logger);
            

            ContainerLocationLogs = new ContainerLocationLogRepository(_context, _logger);
            OrderDetails = new OrderDetailRepository(_context, _logger);
            PaymentHistories = new PaymentHistoryRepository(_context, _logger);




            Storages = new StorageRepository(_context, _logger);
            Orders = new OrderRepository(_context, _logger);
            StorageTypes = new StorageTypeRepository(_context, _logger);
        }
        public async Task CompleteAsync() => await _context.SaveChangesAsync();
    }
}
