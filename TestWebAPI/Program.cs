using ECDsa_Multikey;
using Newtonsoft.Json.Linq;
using System.Text.Json;
using TestWebAPI;
using DataIntegrity;
using ECDsa_2019_Cryptosuite;
using JsonLdSignatures;
using JsonLdSignatures.Purposes;
using Cryptosuite.Core;
using SecurityDocumentLoader;
using Microsoft.AspNetCore.Mvc;
using ECDsa_sd_2023_Cryptosuite;
using ZLogger;
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

app.Run("http://localhost:40443");