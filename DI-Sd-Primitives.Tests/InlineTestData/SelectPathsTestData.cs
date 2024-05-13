using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI_Sd_Primitives.Tests.InlineTestData
{
    public class SelectPaths_ReturnsSelectedPaths_TestData : TheoryData<string, string, string>
    {
        const string CredentialJson = """
            {
              "payload": {
                "name": "Course credential",
                "description": "Diploma in Management",
                "type": [
                  "EducationalOccupationalCredential",
                  "AlumniCredential"
                ],
                "credentialSubject": {
                  "id": "did:key:z6Mkr9f7o82NFLRFTTCWRR1GiZpca22Xf6YKo2zKThrZMA2w",
                  "givenName": "Emma",
                  "familyName": "Tasma",
                  "alumniOf": "Zealopia University"
                },
                "credentialBranding": {
                  "backgroundColor": "#860012",
                  "watermarkImageUrl": "https://static.mattr.global/credential-assets/zealopia/web/watermark.svg"
                },
                "issuer": {
                  "id": "did:web:learn.vii.au01.mattr.global",
                  "name": "Zealopia Business Institute",
                  "iconUrl": "https://static.mattr.global/credential-assets/zealopia/web/logo.svg",
                  "logoUrl": "https://static.mattr.global/credential-assets/zealopia/web/icon.svg"
                },
                "expirationDate": "2024-02-01T08:12:38.156Z",
                "issuanceDate": "2023-02-01T08:12:38.156Z"
              },
              "proofType":"Ed25519Signature2018",
              "tag": "external-identifier",
              "persist": false,
              "revocable": true,
              "includeId": true
            }
            """;
        public SelectPaths_ReturnsSelectedPaths_TestData()
        {
            Add(CredentialJson, "/payload/description", "Diploma in Management");
            Add(CredentialJson, "/payload/credentialSubject/givenName", "Emma");
            Add(CredentialJson, "/payload/credentialSubject/familyName", "Tasma");
            Add(CredentialJson, "/payload/credentialSubject/alumniOf", "Zealopia University");
            Add(CredentialJson, "/payload/credentialBranding/backgroundColor", "#860012");
            Add(CredentialJson, "/payload/type/0", "EducationalOccupationalCredential");
            Add(CredentialJson, "/payload/type/1", "AlumniCredential");
        }
    }
}
