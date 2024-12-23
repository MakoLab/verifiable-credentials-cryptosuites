using TestWebAPI;
using TestWebAPI.Routes;

var builder = WebApplication.CreateBuilder(args);

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

app.Run();