using Cryptosuite.Core.Interfaces;
using DataIntegrity;
using ECDsa_2019_Cryptosuite;
using ECDsa_Multikey;
using JsonLdExtensions;
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
        public IResult VerifyCredential([FromBody] object json, ILogger<VerifierHandlers> logger, ICryptosuiteResolver resolver, IDocumentLoader documentLoader)
        {
            var jsonStr = JsonSerializer.Serialize(json);
            logger.LogDebug("Verifier Request:\n=================");
            logger.LogDebug("{Request}", jsonStr);
            try
            {
                var (document, cryptosuiteName, keypair) = ProcessVerificationRequest(documentLoader, jsonStr);
                var cryptosuite = resolver.GetCryptosuite(cryptosuiteName) ?? throw new ArgumentException("Cryptosuite not found.");
                var suite = new DataIntegrityProof(cryptosuite, keypair.Signer);
                var jss = new JsonLdSignatureService();
                var result = jss.Verify(document, suite, new AssertionMethodPurpose(), documentLoader);
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

        private static (JObject, string, KeyPairInterface) ProcessVerificationRequest(IDocumentLoader documentLoader, string jsonStr)
        {
            if (JObject.Parse(jsonStr)["verifiableCredential"] is not JObject jsonObj)
            {
                throw new ArgumentException("Invalid JSON");
            }
            var cryptosuiteName = GetCryptosuiteName(jsonObj) ?? throw new ArgumentException("Cryptosuite not found in request document.");
            cryptosuiteName = $"{cryptosuiteName}-verify";
            var verificationMethodId = GetVerificationMethodId(jsonObj) ?? throw new ArgumentException("Verification method not found in request document.");
            Uri vmUri;
            vmUri = new Uri(verificationMethodId);
            if (documentLoader.LoadDocument(vmUri)?.Document is not JObject verificationMethodObject)
            {
                throw new ArgumentException("Unable to load Verification Method document.");
            }
            var mvm = verificationMethodObject.ToObject<MultikeyVerificationMethod>() ?? throw new ArgumentException("Verification Method document is not a valid MultikeyVerificationMethod.");
            var keypair = MultikeyService.From(mvm);
            return (jsonObj, cryptosuiteName, keypair);
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
