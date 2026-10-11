using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PSP.APPLICATION.Abstractions;
using PSP.INFRASTRUCTURE.Persistence;
using PSP.INFRASTRUCTURE.Persistence.Repositories;
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

            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<PspDbContext>());

            return services;
        }
    }
}
