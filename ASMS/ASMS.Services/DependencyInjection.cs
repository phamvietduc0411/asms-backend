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
            services.AddScoped<IEmployeeRoleService, EmployeeRoleService>();
            return services;
        }
    }
}

