﻿using ECDsa_2019_Cryptosuite;
using ECDsa_sd_2023_Cryptosuite;
using DataIntegrity;
using ZLogger;

namespace TestWebAPI
{
    public static class Configuration
    {
        public static WebApplicationBuilder ConfigureBuilder(this WebApplicationBuilder builder)
        {
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            builder.Logging.ClearProviders()
                .AddConsole();
            //.AddZLoggerConsole()
            //.AddZLoggerFile($"logs/{DateTime.Now:yy-MM-dd}.log");
            builder.Services.AddSingleton<MockDataProvider>();
            builder.Services.AddCryptosuiteResolver(resolver =>
            {
                resolver.RegisterCryptosuiteType(ECDsa2019Cryptosuite.TypeName, typeof(ECDsa2019Cryptosuite));
                resolver.RegisterCryptosuiteType(ECDsaSd2023CreateProofCryptosuite.TypeName, typeof(ECDsaSd2023CreateProofCryptosuite));
                resolver.RegisterCryptosuiteType(ECDsaSd2023DisclosureCryptosuite.TypeName, typeof(ECDsaSd2023DisclosureCryptosuite));
                resolver.RegisterCryptosuiteType(ECDsaSd2023VerifyCryptosuite.TypeName, typeof(ECDsaSd2023VerifyCryptosuite));

            });
            builder.Services.AddSingleton<IDidDocumentCreator, DidDocumentCreator>();
            return builder;
        }
    }
}
