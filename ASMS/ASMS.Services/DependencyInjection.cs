using ASMS.Services.Interfaces;
using ASMS.Services.Mappings;
using ASMS.Services.Services;
using ASMS.Services.Utilities;
using Microsoft.Extensions.DependencyInjection;


namespace ASMS.Services

{
    public static class DependencyInjection
    {
        public static IServiceCollection ConfigureServicesLayers(this IServiceCollection services)
        {
            services.AddAutoMapper(cfg => { }, typeof(MappingProfiles).Assembly);
            services.AddScoped<IBuildingService, BuildingService>();
            services.AddScoped<IEmployeeRoleService, EmployeeRoleService>();
            services.AddScoped<IServiceService, ServiceService>();
            services.AddScoped<ITrackingHistoryService, TrackingHistoryService>();
            services.AddScoped<IWorkflowStepService, WorkflowStepService>();
            services.AddScoped<IWorkflowTemplateService, WorkflowTemplateService>();
            services.AddScoped<IBuildingService, BuildingService>();
            services.AddScoped<IProductTypeService, ProductTypeService>();
            services.AddScoped<IFloorService, FloorService>();
            services.AddScoped<IContainerTypeService, ContainerTypeService>();
            services.AddScoped<IShelfService, ShelfService>();
            services.AddScoped<IContainerLocationLogService, ContainerLocationLogService>();
            services.AddScoped<IContainerService, ContainerService>();
            services.AddScoped<IOrderDetailService, OrderDetailService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IStorageService, StorageService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IStorageTypeService, StorageTypeService>();
            services.AddScoped<ICLPService, CLPService>();
            services.AddScoped<TokenService>();
            services.AddScoped<IPaymentHistoryService, PaymentHistoryService>();
            services.AddScoped<IImageUrlService, ImageUrlService>();
            return services;
        }
    }
}

