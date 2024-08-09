using ZLogger;

namespace TestWebAPI
{
    public static class Configuration
    {
        public static WebApplicationBuilder ConfigureBuilder(this WebApplicationBuilder builder)
        {
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Logging.ClearProviders()
                .AddZLoggerConsole()
                .AddZLoggerFile($"logs/{DateTime.Now:yy-MM-dd}.log");
            return builder;
        }
    }
}
