﻿using DataIntegrity;
using ECDsa_2019_Cryptosuite;
using ECDsa_Multikey;
using ECDsa_sd_2023_Cryptosuite;
using JsonLdSignatures;
using JsonLdSignatures.Purposes;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Text.Json;

namespace TestWebAPI.Routes
{
    public static class IssuerRoutes
    {
        public static void RegisterIssuerRoutes(this IEndpointRouteBuilder erb)
        {
            var handler = new IssuerHandlers();

            erb.MapGet("/issuers", handler.GetIssuers)
                .WithName("Issuers")
                .WithOpenApi();

            erb.MapGet("/issuers/{id}", handler.GetVerificationMethod)
                .WithName("VerificationMethod")
                .WithOpenApi();

            erb.MapPost("/issuers/credentials/issue", handler.IssueCredential)
                .WithName("Issuer")
                .WithOpenApi();

            erb.MapPost("/issuers/sd/credentials/issue", handler.IssueSdCredential)
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

        public IResult GetVerificationMethod(string id, ILogger<IssuerHandlers> logger, MockDataProvider mdp)
        {
            logger.LogDebug("Issuer request: GET Verification Method");
            logger.LogDebug("Issuer response: {Verification Method Document}", mdp.GetVerificationMethodDocument(id));
            return Results.Content(mdp.GetVerificationMethodDocument(id), contentType: "application/json", statusCode: 200);
        }

        public IResult IssueCredential([FromBody] object json, ILogger<IssuerHandlers> logger, MockDataProvider mdp)
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
            var loader = new SecurityDocumentLoader.SecurityDocumentLoader();
            try
            {
                var signed = jsonLd.Sign(document, suite, new AssertionMethodPurpose(new Cryptosuite.Core.Controller { Id = mdp.VerificationMethodId }, date), loader);
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

        public IResult IssueSdCredential([FromBody] object json, ILogger<IssuerHandlers> logger, MockDataProvider mdp)
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
            var crypto = new ECDsaSd2023CreateProofCryptosuite();
            var suite = new DataIntegrityProof(crypto, keypair.Signer, date);
            var jsonLd = new JsonLdSignatureService();
            var loader = new SecurityDocumentLoader.SecurityDocumentLoader();
            try
            {
                var signed = jsonLd.Sign(document, suite, new AssertionMethodPurpose(new Cryptosuite.Core.Controller { Id = mdp.VerificationMethodId }, date), loader);
                logger.LogDebug("Sd Issuer response:\n===================");
                logger.LogDebug("{Response}", jsonStr);
                jsonStr = signed.ToString();
                return Results.Json(JsonDocument.Parse(jsonStr));
            }
            catch (Exception e)
            {
                logger.LogError("Error:\n======\n{Error message}", e.Message);
                return Results.BadRequest(e.Message);
            }
        }
    }
}
