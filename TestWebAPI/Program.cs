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

app.MapGet("/issuers/" + MockData.PublicKeyMultibase, () =>
{
    return Results.Content(MockData.GetVerificationMethodDocument(), contentType: "application/json", statusCode: 200);
});

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
    var jsonObj = JObject.Parse(jsonStr).ParseJson();
    if (jsonObj is null)
    {
        return Results.BadRequest("Invalid JSON");
    }
    var keypair = Multikey.From(new MultikeyModel
    {
        PublicKeyMultibase = MockData.PublicKeyMultibase,
        SecretKeyMultibase = MockData.SecretKeyMultibase,
        Controller = MockData.Controller
    });
    var crypto = new ECDsa2019Cryptosuite();
    var suite = new DataIntegrityProof(crypto, keypair.Signer);
    var jsonld = new JsonLdSignatureService();
    var loader = new TestWebDocumentLoader();
    try
    {
        var result = jsonld.Verify(jsonObj, suite, new AssertionMethodPurpose(), loader);
        app.Logger.LogDebug("Verify");
        var response = jsonld.ToJsonResult(result).ToString();
        app.Logger.LogDebug("{Response}", response);
        return Results.Content(response, contentType: "application/json", statusCode: 200);
    }
    catch (Exception e)
    {
        var response = jsonld.ToJsonResult(e.Message, System.Net.HttpStatusCode.BadRequest).ToString();
        app.Logger.LogError("{Exception message}", e.Message);
        app.Logger.LogError("{Response}",response);
        return Results.Content(response, contentType: "application/json", statusCode: 400);
    }
})
.WithName("Verifier")
.WithOpenApi();

app.Run("http://localhost:40443");

public static class RequestParser
{
    public static JObject? ParseJson(this JObject json)
    {
        return json["verifiableCredential"] as JObject;
    }
}