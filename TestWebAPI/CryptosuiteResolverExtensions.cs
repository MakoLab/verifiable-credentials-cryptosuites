using Microsoft.Extensions.DependencyInjection;
using System;

namespace TestWebAPI
{
    public static class CryptosuiteResolverExtensions
    {
        public static IServiceCollection AddCryptosuiteResolver(this IServiceCollection services, Action<CryptosuiteResolver> configure)
        {
            services.AddSingleton<ICryptosuiteResolver, CryptosuiteResolver>();
            var serviceProvider = services.BuildServiceProvider();
            var resolver = serviceProvider.GetRequiredService<ICryptosuiteResolver>() as CryptosuiteResolver
                ?? throw new InvalidOperationException("CryptosuiteResolver not registered");
            configure(resolver);
            return services;
        }
    }
}