using ASMS.Services.Interfaces;
using ASMS.Services.Mappings;
using ASMS.Services.Services;
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
            services.AddScoped<IStorageBlockService, StorageBlockService>();
            services.AddScoped<ITrackingHistoryService, TrackingHistoryService>();
            services.AddScoped<IWorkflowStepService, WorkflowStepService>();
            services.AddScoped<IWorkflowTemplateService, WorkflowTemplateService>();
            services.AddScoped<IBuildingService, BuildingService>();
            services.AddScoped<IProductTypeService, ProductTypeService>();
            services.AddScoped<IFloorService, FloorService>();
            services.AddScoped<IFloorBlockService, FloorBlockService>();
            services.AddScoped<IContainerLocationLogService, ContainerLocationLogService>();
            return services;
        }
    }
}

