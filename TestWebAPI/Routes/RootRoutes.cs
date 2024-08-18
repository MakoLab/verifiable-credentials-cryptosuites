namespace TestWebAPI.Routes
{
    public static class RootRoutes
    {
        public static RouteHandlerBuilder RegisterHomeRoute(this IEndpointRouteBuilder erb)
        {
            var handler = new RootHandlers();
            var builder = erb.MapGet("/", handler.GetHome)
             .WithName("Root")
             .WithOpenApi();
            return builder;
        }
    }

    public class RootHandlers
    {
        public string GetHome()
        {
            return "Hello World!";
        }
    }
}
