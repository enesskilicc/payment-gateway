using Microsoft.Extensions.DependencyInjection;
using PSP.APPLICATION.ApiKeys;
using PSP.APPLICATION.Authentication;
using PSP.APPLICATION.Merchants;
using PSP.APPLICATION.Payments;

namespace PSP.APPLICATION
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<PaymentService>();
            services.AddScoped<ApiKeyService>();
            services.AddScoped<MerchantService>();
            services.AddScoped<ApiKeyAuthenticator>();

            return services;
        }
    }
}
