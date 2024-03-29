﻿using Cryptosuite.Core;
using ECDsa_Multikey;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDsa_2019_Cryptosuite.Tests
{
    public class MockData
    {
        private const string publicKeyMultibase = "zDnaekGZTbQBerwcehBSXLqAg6s55hVEBms1zFy89VHXtJSa9";
        private const string secretKeyMultibase = "z42tqZ5smVag3DtDhjY9YfVwTMyVHW6SCHJi2ZMrD23DGYS3";
        private readonly string id = $"{Controller}#{publicKeyMultibase}";
        private const string credString = """
            {
              "@context": [
                "https://www.w3.org/2018/credentials/v1",
                {
                  "AlumniCredential": "https://schema.org#AlumniCredential",
                  "alumniOf": "https://schema.org#alumniOf"
                },
                "https://w3id.org/security/data-integrity/v1"
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
        public readonly MultikeyModel MockPublicEcdsaMultikey;
        public readonly MultikeyModel EcdsaMultikeyKeyPair;
        public readonly MultikeyModel EcdsaSecp256KeyPair;
        public readonly MultikeyModel ControllerDocEcdsaMultikey;
        public readonly JObject Credential = JObject.Parse(credString);

        public MockData()
        {
            MockPublicEcdsaMultikey = new MultikeyModel
            {
                Context = new JValue("https://w3id.org/security/multikey/v1"),
                Type = "Multikey",
                Controller = Controller,
                Id = id,
                PublicKeyMultibase = publicKeyMultibase,
            };

            EcdsaMultikeyKeyPair = new MultikeyModel
            {
                Context = new JValue("https://w3id.org/security/multikey/v1"),
                Type = "Multikey",
                Controller = Controller,
                Id = id,
                PublicKeyMultibase = publicKeyMultibase,
                SecretKeyMultibase = secretKeyMultibase,
            };

            EcdsaSecp256KeyPair = new MultikeyModel
            {
                Type = "EcdsaSecp256r1VerificationKey2019",
                Controller = Controller,
                PublicKeyMultibase = publicKeyMultibase,
                SecretKeyMultibase = secretKeyMultibase,
            };

            ControllerDocEcdsaMultikey = new MultikeyModel
            {
                Context = new JArray { "https://www.w3.org/ns/did/v1", "https://w3id.org/security/multikey/v1" },
                Controller = Controller,
                Id = "https://example.edu/issuers/565049",
                AssertionMethod = new List<VerificationMethod> { MockPublicEcdsaMultikey },
            };
        }
    }
}
