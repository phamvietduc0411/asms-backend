using ASMS.Repositories.Infrastructures;
using Microsoft.Extensions.DependencyInjection;

namespace ASMS.Services

{
    public static class DependencyInjection
    {
        public static IServiceCollection ConfigureRepositoryServices(
            this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}

