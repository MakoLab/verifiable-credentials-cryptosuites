using Microsoft.Extensions.DependencyInjection;
using System;

namespace TestWebAPI
{
    public static class CryptosuiteResolverExtensions
    {
        public static IServiceCollection AddCryptosuiteResolver(this IServiceCollection services, Action<CryptosuiteResolver> configure)
        {
            services.AddSingleton<ICryptosuiteResolver, CryptosuiteResolver>(sp =>
            {
                var resolver = new CryptosuiteResolver(sp.GetRequiredService<ILogger<CryptosuiteResolver>>());
                configure(resolver);
                return resolver;
            });
            return services;
        }
    }

}