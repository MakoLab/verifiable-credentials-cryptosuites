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
        public IResult VerifyCredential([FromBody] object json, ILogger<VerifierHandlers> logger, MockDataProvider mdp, ICryptosuiteResolver resolver)
        {
            var jsonStr = JsonSerializer.Serialize(json);
            logger.LogDebug("Verifier Request:\n=================");
            logger.LogDebug("{Request}", jsonStr);
            if (JObject.Parse(jsonStr)["verifiableCredential"] is not JObject jsonObj)
            {
                return Results.BadRequest("Invalid JSON");
            }
            var cryptosuiteName = GetCryptosuiteName(jsonObj);
            if (cryptosuiteName is null)
            {
                return Results.BadRequest("Cryptosuite not found in request document.");
            }
            cryptosuiteName = $"{cryptosuiteName}-verify";
            var cryptosuite = resolver.GetCryptosuite(cryptosuiteName);
            if (cryptosuite is null)
            {
                return Results.BadRequest("Cryptosuite not recognized.");
            }
            var verificationMethodId = GetVerificationMethodId(jsonObj);
            if (verificationMethodId is null)
            {
                return Results.BadRequest("Verification method not found in request document.");
            }
            Uri vmUri;
            try
            {
                vmUri = new Uri(verificationMethodId);
            }
            catch (Exception)
            {
                return Results.BadRequest("Verification method is not a valid URI.");
            }
            var documentLoader = new VCDIDocumentLoader();
            if (documentLoader.LoadDocument(vmUri)?.Document is not JObject verificationMethodObject)
            {
                return Results.BadRequest("Unable to load Verification Method document.");
            }
            var mvm = verificationMethodObject.ToObject<MultikeyVerificationMethod>();
            if (mvm is null)
            {
                return Results.BadRequest("Verification Method document is not a valid MultikeyVerificationMethod.");
            }
            var keypair = MultikeyService.From(mvm);
            var suite = new DataIntegrityProof(cryptosuite, keypair.Signer);
            var jss = new JsonLdSignatureService();
            try
            {
                var result = jss.Verify(jsonObj, suite, new AssertionMethodPurpose(), documentLoader);
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

        private static string? GetCryptosuiteName(JObject jObject)
        {
            return jObject["proof"]?["cryptosuite"]?.ToString();
        }

        private static string? GetVerificationMethodId(JObject jObject)
        {
            return jObject["proof"]?["verificationMethod"]?.ToString();
        }
    }
}
