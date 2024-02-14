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

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Logging.AddConsole();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () =>
{
    return "Hello World!";
})
.WithName("Root")
.WithOpenApi();

app.MapPost("/issuers/credentials/issue", ([FromBody] object json) =>
{
    // Deserialize the JSON object

    var jsonStr = JsonSerializer.Serialize(json);
    var jsonObj = JObject.Parse(jsonStr);
    var keypair = Multikey.From(new MultikeyModel
    {
        PublicKeyMultibase = MockData.PublicKeyMultibase,
        SecretKeyMultibase = MockData.SecretKeyMultibase,
        Controller = MockData.Controller
    });
    var date = DateTime.Parse("2023-03-01T21:29:24Z");
    var crypto = new ECDsa2019Cryptosuite();
    var suite = new DataIntegrityProof(crypto, keypair.Signer, date);
    var jsonLd = new JsonLdSignatureService();
    var loader = new SecurityDocumentLoader.SecurityDocumentLoader();
    try
    {
        var signed = jsonLd.Sign(jsonObj, suite, new AssertionMethodPurpose(new Cryptosuite.Core.Controller { Id = MockData.Id }, date), loader);
        app.Logger.LogDebug("Issue");
        jsonStr = signed.ToString();
        return Results.Json(JsonDocument.Parse(jsonStr));
    }
    catch (Exception e)
    {
        app.Logger.LogError(e.Message);
        return Results.BadRequest(e.Message);
    }
    
})
.WithName("Issuer")
.WithOpenApi();

app.MapPost("/verifiers/credentials/verify", ([FromBody] object json) =>
{
    var jsonStr = JsonSerializer.Serialize(json);
    var jsonObj = JObject.Parse(jsonStr);
    var keypair = Multikey.From(new MultikeyModel
    {
        PublicKeyMultibase = MockData.PublicKeyMultibase,
        SecretKeyMultibase = MockData.SecretKeyMultibase,
        Controller = MockData.Controller
    });
    var crypto = new ECDsa2019Cryptosuite();
    var suite = new DataIntegrityProof(crypto, keypair.Signer);
    var jsonld = new JsonLdSignatureService();
    var loader = new SecurityDocumentLoader.SecurityDocumentLoader();
    try
    {
        var result = jsonld.Verify(jsonObj, suite, new AssertionMethodPurpose(), loader);
        app.Logger.LogDebug("Verify");
        jsonObj = jsonld.ToJsonResult(result);
        return Results.Json(jsonObj);
    }
    catch (Exception e)
    {
        app.Logger.LogError(e.Message);
        return Results.BadRequest(e.Message);
    }
    
})
.WithName("Verifier")
.WithOpenApi();

app.Run("http://localhost:40443");