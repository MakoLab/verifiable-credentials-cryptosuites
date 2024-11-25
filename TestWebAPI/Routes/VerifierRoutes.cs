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
        public IResult VerifyCredential([FromBody] object json, ILogger<VerifierHandlers> logger, MockDataProvider mdp, IDidDocumentCreator didDocumentCreator)
        {
            var jsonStr = JsonSerializer.Serialize(json);
            logger.LogDebug("Verifier Request:\n=================");
            logger.LogDebug("{Request}", jsonStr);
            if (JObject.Parse(jsonStr)["verifiableCredential"] is not JObject jsonObj)
            {
                return Results.BadRequest("Invalid JSON");
            }
            var keypair = MultikeyService.From(new MultikeyVerificationMethod
            {   
                Id = mdp.VerificationMethodId,
                Controller = mdp.ControllerId,
                PublicKeyMultibase = mdp.PublicKeyMultibase,
                SecretKeyMultibase = mdp.SecretKeyMultibase,
            });
            var crypto = new ECDsa2019Cryptosuite();
            var suite = new DataIntegrityProof(crypto, keypair.Signer);
            var jss = new JsonLdSignatureService();
            var loader = new SecurityDocumentLoader.SecurityDocumentLoader(didDocumentCreator);
            try
            {
                var result = jss.Verify(jsonObj, suite, new AssertionMethodPurpose(), loader);
                var response = JsonLdSignatureService.ToJsonResult(result).ToString();
                logger.LogDebug("Verifier response:\n==================");
                logger.LogDebug("{Response}", response);
                var statusCode = result.Any(r => r.IsSuccess) ? 200 : 400;
                return Results.Content(response, contentType: "application/json", statusCode: statusCode);
            }
            catch (Exception e)
            {
                var response = JsonLdSignatureService.ToJsonResult(e.Message, System.Net.HttpStatusCode.BadRequest).ToString();
                logger.LogError("Error:\n======\n{Exception message}", e.Message);
                logger.LogDebug("Verifier response:\n==================");
                logger.LogError("{Response}", response);
                return Results.BadRequest(response);
            }
        }
    }
}
