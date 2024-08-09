using DataIntegrity;
using ECDsa_2019_Cryptosuite;
using ECDsa_Multikey;
using JsonLdSignatures;
using JsonLdSignatures.Purposes;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Text.Json;

namespace TestWebAPI.Routes
{
    public static class VerifierRoutes
    {
        public static void RegisterVerifierRoutes(this IEndpointRouteBuilder erb)
        {
            var handler = new VerifierHandlers();

            erb.MapPost("/verifiers/credentials/verify", handler.VerifyCredential)
                .WithName("Verifier")
                .WithOpenApi();
        }
    }

    public class VerifierHandlers
    {
        public IResult VerifyCredential([FromBody] object json, ILogger<VerifierHandlers> logger)
        {
            var jsonStr = JsonSerializer.Serialize(json);
            logger.LogDebug("Verifier Request:\n=================");
            logger.LogDebug("{Request}", jsonStr);
            if (JObject.Parse(jsonStr)["verifiableCredential"] is not JObject jsonObj)
            {
                return Results.BadRequest("Invalid JSON");
            }
            var keypair = MultikeyService.From(new MultikeyVerificationMethod(MockData.VerificationMethodId, MockData.ControllerId)
            {
                PublicKeyMultibase = MockData.PublicKeyMultibase,
                SecretKeyMultibase = MockData.SecretKeyMultibase,
            });
            var crypto = new ECDsa2019Cryptosuite();
            var suite = new DataIntegrityProof(crypto, keypair.Signer);
            var jsonld = new JsonLdSignatureService();
            var loader = new TestWebDocumentLoader();
            try
            {
                var result = jsonld.Verify(jsonObj, suite, new AssertionMethodPurpose(), loader);
                var response = jsonld.ToJsonResult(result).ToString();
                logger.LogDebug("Verifier response:\n==================");
                logger.LogDebug("{Response}", response);
                return Results.Content(response, contentType: "application/json", statusCode: 200);
            }
            catch (Exception e)
            {
                var response = jsonld.ToJsonResult(e.Message, System.Net.HttpStatusCode.BadRequest).ToString();
                logger.LogError("Error:\n======\n{Exception message}", e.Message);
                logger.LogDebug("Verifier response:\n==================");
                logger.LogError("{Response}", response);
                return Results.Content(response, contentType: "application/json", statusCode: 400);
            }
        }
    }
}
