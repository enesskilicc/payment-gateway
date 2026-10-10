using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PSP.INFRASTRUCTURE.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace PSP.INFRASTRUCTURE
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<PspDbContext>(options => {
                options.UseNpgsql(connectionString).UseSnakeCaseNamingConvention();
            });

            return services;
        }
    }
}
