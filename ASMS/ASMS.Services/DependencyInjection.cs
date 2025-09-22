using ASMS.Services.Interfaces;
using ASMS.Services.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ASMS.Services

{
    public static class DependencyInjection
    {
        public static IServiceCollection ConfigureServicesLayers(this IServiceCollection services)
        {
            services.AddScoped<IEmployeeRoleService, EmployeeRoleService>();
            return services;
        }
    }
}

