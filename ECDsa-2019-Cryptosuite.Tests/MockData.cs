﻿using Cryptosuite.Core;
using Cryptosuite.Core.ControllerDocuments;
using ECDsa_Multikey;
using Newtonsoft.Json.Linq;

namespace ECDsa_2019_Cryptosuite.Tests
{
    public class MockData
    {
        private const string publicKeyMultibase = "zDnaekGZTbQBerwcehBSXLqAg6s55hVEBms1zFy89VHXtJSa9";
        private const string secretKeyMultibase = "z42tqZ5smVag3DtDhjY9YfVwTMyVHW6SCHJi2ZMrD23DGYS3";
        private readonly string keyId = $"{Controller}#{publicKeyMultibase}";
        private const string credString = """
            {
              "@context": [
                "https://www.w3.org/2018/credentials/v1",
                {
                  "AlumniCredential": "https://schema.org#AlumniCredential",
                  "alumniOf": "https://schema.org#alumniOf"
                },
              ],
              "id": "http://example.edu/credentials/1872",
              "type": [
                "VerifiableCredential",
                "AlumniCredential"
              ],
              "issuer": "https://example.edu/issuers/565049",
              "issuanceDate": "2010-01-01T19:23:24Z",
              "credentialSubject": {
                "id": "https://example.edu/students/alice",
                "alumniOf": "Example University"
              }
            }
            """;

        internal const string Controller = "https://example.edu/issuers/565049";
        public readonly MultikeyVerificationMethod MockPublicEcdsaMultikey;
        public readonly ControllerDocument ControllerDocEcdsaMultikey;
        public readonly JObject Credential = JObject.Parse(credString);

        public MockData()
        {
            MockPublicEcdsaMultikey = new MultikeyVerificationMethod()
            {
                Id = keyId,
                Controller = Controller,
                PublicKeyMultibase = publicKeyMultibase,
                SecretKeyMultibase = secretKeyMultibase,
            };

            ControllerDocEcdsaMultikey = new ControllerDocument("https://example.edu/issuers/565049")
            {
                Context = new JArray { Contexts.DidContextV1Url, "https://w3id.org/security/multikey/v1", Contexts.CredentialsContextV2Url },
                VerificationMethod = [MockPublicEcdsaMultikey],
                AssertionMethod = [keyId],
            };
        }
    }
}
