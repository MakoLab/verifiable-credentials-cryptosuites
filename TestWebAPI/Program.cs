using TestWebAPI;
using TestWebAPI.Routes;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.ConfigureBuilder();

var app = builder.Build();

// Configure the HTTPS request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.RegisterHomeRoute();
app.RegisterIssuerRoutes();
app.RegisterVerifierRoutes();

app.Run("https://localhost:40443");