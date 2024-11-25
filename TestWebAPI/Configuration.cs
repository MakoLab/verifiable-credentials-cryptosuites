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
            builder.Services.AddSingleton<IDidDocumentCreator, DidDocumentCreator>();
            return builder;
        }
    }
}
