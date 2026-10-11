using Microsoft.Extensions.DependencyInjection;
using PSP.APPLICATION.Payments;
using System;
using System.Collections.Generic;
using System.Text;

namespace PSP.APPLICATION
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<PaymentService>();

            return services;
        }
    }
}
