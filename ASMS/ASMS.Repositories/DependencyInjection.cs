using ASMS.Repositories.Data;
using ASMS.Repositories.Infrastructures;
using ASMS.Repositories.Interfaces;
using ASMS.Repositories.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace ASMS.Services

{
    public static class DependencyInjection
    {
        public static IServiceCollection ConfigureRepositoryServices(
            this IServiceCollection services)
        {
            services.AddScoped<VstorageContext>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IBuildingRepository, BuildingRepository>();
            return services;
        }
    }
}

