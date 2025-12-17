using ASMS.Services.Interfaces;
using ASMS.Services.Mappings;
using ASMS.Services.Services;
using ASMS.Services.Setting;
using ASMS.Services.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Net.payOS;



namespace ASMS.Services

{
    public static class DependencyInjection
    {
        public static IServiceCollection ConfigureServicesLayers(this IServiceCollection services, IConfiguration configuration)
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
            services.AddScoped<IShelfTypeService, ShelfTypeService>();
            services.AddScoped<IPasswordService, PasswordService>();
            services.AddScoped<IPayOSService, PayOSService>();
            services.AddScoped<IOrderStatusService, OrderStatusService>();
            services.AddScoped<IDistanceService, DistanceService>();
            services.AddScoped<IOrderMaintenanceService, OrderMaintenanceService>();
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddScoped<IPricingService, PricingService>();
            services.AddScoped<IContactService, ContactService>();
            services.AddScoped<IBusinessRuleService, BusinessRuleService>();
            services.AddScoped<IShortLinkService, ShortLinkService>();
            services.Configure<PayOSSettings>(configuration.GetSection("PayOSSettings"));

            services.AddSingleton(sp =>
            {
                var settings = sp.GetRequiredService<IOptions<PayOSSettings>>().Value;
                if (string.IsNullOrEmpty(settings.ClientId) ||
                    string.IsNullOrEmpty(settings.ApiKey) ||
                    string.IsNullOrEmpty(settings.ChecksumKey))
                {
                    throw new InvalidOperationException("PayOS configuration is missing or incomplete");
                }
                return new PayOS(settings.ClientId, settings.ApiKey, settings.ChecksumKey);
            });

            return services;
        }
    }
}

