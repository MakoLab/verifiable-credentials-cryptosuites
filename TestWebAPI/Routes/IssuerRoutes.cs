using DataIntegrity;
using ECDsa_2019_Cryptosuite;
using ECDsa_Multikey;
using ECDsa_sd_2023_Cryptosuite;
using JsonLdExtensions;
using JsonLdSignatures;
using JsonLdSignatures.Purposes;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using SecurityTestDocumentLoader;
using System.Text.Json;

namespace TestWebAPI.Routes
{
    public static class IssuerRoutes
    {
        public static void RegisterIssuerRoutes(this IEndpointRouteBuilder erb)
        {
            var handlers = new IssuerHandlers();

            erb.MapGet("/issuers", handlers.GetIssuers)
                .WithName("Issuers")
                .WithOpenApi();

            erb.MapGet("/issuers/{id}", handlers.GetController)
                .WithName("Controller")
                .WithOpenApi();

            erb.MapGet("/issuers/{cId}/{vId}", handlers.GetVerificationMethod)
                .WithName("VerificationMethod")
                .WithOpenApi();

            erb.MapPost("/issuers/credentials/issue", handlers.IssueCredential)
                .WithName("Issuer")
                .WithOpenApi();

            erb.MapPost("/issuers/sd/credentials/issue", handlers.IssueSdCredential)
                .WithName("IssuerSd")
                .WithOpenApi();
        }
    }

    public class IssuerHandlers
    {
        public IResult GetIssuers(ILogger<IssuerHandlers> logger, MockDataProvider mdp)
        {
            logger.LogDebug("Issuer request: GET Controller Document");
            logger.LogDebug("Issuer response: {Controller Document}", mdp.GetControllerDocument());
            return Results.Content(mdp.GetControllerDocument(), contentType: "application/json", statusCode: 200);
        }

        public IResult GetController(string id, ILogger<IssuerHandlers> logger, MockDataProvider mdp)
        {
            logger.LogDebug("Issuer request: GET Controller Document");
            logger.LogDebug("Issuer response: {Controller Document}", mdp.GetControllerDocument());
            return Results.Content(mdp.GetControllerDocument(), contentType: "application/json", statusCode: 200);
        }

        public IResult GetVerificationMethod(string cId, string vId, ILogger<IssuerHandlers> logger, MockDataProvider mdp)
        {
            logger.LogDebug("Issuer request: GET Verification Method");
            logger.LogDebug("Issuer response: {Verification Method Document}", mdp.GetVerificationMethodDocument(cId));
            return Results.Content(mdp.GetVerificationMethodDocument(cId), contentType: "application/json", statusCode: 200);
        }

        public IResult IssueCredential([FromBody] object json, ILogger<IssuerHandlers> logger, MockDataProvider mdp, IDocumentLoader documentLoader)
        {
            var jsonStr = JsonSerializer.Serialize(json);
            logger.LogDebug("Issuer request:\n===============");
            logger.LogDebug("{Request}", jsonStr);
            var jsonObj = JObject.Parse(jsonStr);
            if (jsonObj["credential"] as JObject is null)
            {
                return Results.BadRequest("Invalid request: 'credential' property is required");
            }
            var document = (jsonObj["credential"] as JObject)!;
            var keypair = MultikeyService.From(new MultikeyVerificationMethod()
            {
                Id = mdp.VerificationMethodId,
                Controller = mdp.ControllerId,
                PublicKeyMultibase = mdp.PublicKeyMultibase,
                SecretKeyMultibase = mdp.SecretKeyMultibase,
            });
            var date = DateTime.Parse("2023-03-01T21:29:24Z");
            var crypto = new ECDsa2019Cryptosuite();
            var suite = new DataIntegrityProof(crypto, keypair.Signer, date);
            var jsonLd = new JsonLdSignatureService();
            try
            {
                var signed = jsonLd.Sign(document, suite, new AssertionMethodPurpose(new Cryptosuite.Core.Controller { Id = mdp.VerificationMethodId }, date), documentLoader);
                jsonStr = signed.ToString();
                logger.LogDebug("Issuer response:\n================");
                logger.LogDebug("{Response}", jsonStr);
                return Results.Json(JsonDocument.Parse(jsonStr));
            }
            catch (Exception e)
            {
                logger.LogError("Error:\n======\n{Error message}", e.Message);
                return Results.BadRequest(e.Message);
            }
        }

        public IResult IssueSdCredential([FromBody] object json, ILogger<IssuerHandlers> logger, MockDataProvider mdp, IDocumentLoader documentLoader)
        {
            var jsonStr = JsonSerializer.Serialize(json);
            logger.LogDebug("Sd Issuer request:\n==================");
            logger.LogDebug("{Request}", jsonStr);
            var jsonObj = JObject.Parse(jsonStr);
            if (jsonObj["credential"] as JObject is null)
            {
                return Results.BadRequest("Invalid request: 'credential' property is required");
            }
            var document = (jsonObj["credential"] as JObject)!;
            var keypair = MultikeyService.From(new MultikeyVerificationMethod()
            {
                Id = mdp.VerificationMethodId,
                Controller = mdp.ControllerId,
                PublicKeyMultibase = mdp.PublicKeyMultibase,
                SecretKeyMultibase = mdp.SecretKeyMultibase,
            });
            var date = DateTime.Parse("2023-03-01T21:29:24Z");
            var mandatoryPointers = GetMandatoryPointers(jsonObj);
            var crypto = new ECDsaSd2023CreateProofCryptosuite(mandatoryPointers);
            var suite = new DataIntegrityProof(crypto, keypair.Signer, date);
            var jsonLd = new JsonLdSignatureService();
            try
            {
                var signed = jsonLd.Sign(document, suite, new AssertionMethodPurpose(new Cryptosuite.Core.Controller { Id = mdp.VerificationMethodId }, date), documentLoader);
                jsonStr = signed.ToString();
                logger.LogDebug("Sd Issuer response:\n===================");
                logger.LogDebug("{Response}", jsonStr);
                return Results.Json(JsonDocument.Parse(jsonStr));
            }
            catch (Exception e)
            {
                logger.LogError("Error:\n======\n{Error message}", e.Message);
                return Results.BadRequest(e.Message);
            }
        }

        private static IList<string> GetMandatoryPointers(JObject jsonObj)
        {
            if (jsonObj["options"]?["mandatoryPointers"] is not JArray mandatoryPointers)
            {
                return [];
            }
            return [.. mandatoryPointers.Select(p => p.ToString())];
        }
    }
}
