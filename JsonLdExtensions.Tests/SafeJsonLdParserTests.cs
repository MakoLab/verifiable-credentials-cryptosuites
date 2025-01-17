using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JsonLdExtensions;
using JsonLdExtensions.Canonicalization;
using Newtonsoft.Json;
using VDS.RDF;

namespace JsonLdExtensions.Tests
{
    public class SafeJsonLdParserTests
    {
        const string ValidCred = """
            {
                "@context": [
                    "https://www.w3.org/ns/credentials/v2",
                    {
                        "@protected": true,
                        "DriverLicenseCredential": "urn:example:DriverLicenseCredential",
                        "DriverLicense": {
                            "@id": "urn:example:DriverLicense",
                            "@context": {
                                "@protected": true,
                                "id": "@id",
                                "type": "@type",
                                "documentIdentifier": "urn:example:documentIdentifier",
                                "dateOfBirth": "urn:example:dateOfBirth",
                                "expirationDate": "urn:example:expiration",
                                "issuingAuthority": "urn:example:issuingAuthority"
                            }
                        },
                        "driverLicense": {
                            "@id": "urn:example:driverLicense",
                            "@type": "@id"
                        }
                    }
                ],
                "id": "urn:uuid:f16eb9aa-ac44-4979-8d62-da3536baf649",
                "type": [
                    "VerifiableCredential",
                    "DriverLicenseCredential"
                ],
                "credentialSubject": {
                    "id": "urn:uuid:1a0e4ef5-091f-4060-842e-18e519ab9440",
                    "driverLicense": {
                        "type": "DriverLicense",
                        "documentIdentifier": "T21387yc328c7y32h23f23",
                        "dateOfBirth": "01-01-1990",
                        "expirationDate": "01-01-2030",
                        "issuingAuthority": "VA"
                    }
                },
                "issuer": "https://localhost:40443/issuers/zDnaeipTBN8tgRmkjZWaQSBFj4Ub3ywWP6vAsgGET922nkvZz"
            }
            """;
        const string InvalidJson = "Invalid JSON";
        const string InvalidType = """
                    {
                "@context": [
                    "https://www.w3.org/ns/credentials/v2",
                    {
                        "@protected": true,
                        "DriverLicenseCredential": "urn:example:DriverLicenseCredential",
                        "DriverLicense": {
                            "@id": "urn:example:DriverLicense",
                            "@context": {
                                "@protected": true,
                                "id": "@id",
                                "type": "@type",
                                "documentIdentifier": "urn:example:documentIdentifier",
                                "dateOfBirth": "urn:example:dateOfBirth",
                                "expirationDate": "urn:example:expiration",
                                "issuingAuthority": "urn:example:issuingAuthority"
                            }
                        },
                        "driverLicense": {
                            "@id": "urn:example:driverLicense",
                            "@type": "@id"
                        }
                    }
                ],
                "id": "urn:uuid:f16eb9aa-ac44-4979-8d62-da3536baf649",
                "type": [
                    "VerifiableCredential",
                    "DriverLicenseCredential",
                    "InvalidType"
                ],
                "credentialSubject": {
                    "id": "urn:uuid:1a0e4ef5-091f-4060-842e-18e519ab9440",
                    "driverLicense": {
                        "type": "DriverLicense",
                        "documentIdentifier": "T21387yc328c7y32h23f23",
                        "dateOfBirth": "01-01-1990",
                        "expirationDate": "01-01-2030",
                        "issuingAuthority": "VA"
                    }
                },
                "issuer": "https://localhost:40443/issuers/zDnaeipTBN8tgRmkjZWaQSBFj4Ub3ywWP6vAsgGET922nkvZz"
            }
            """;

        [Fact]
        public void LoadFromString_ParsesProperlyValidCredential()
        {
            var parser = new SafeJsonLdParser();
            var ts = new TripleStore();
            parser.Load(ts, new StringReader(ValidCred));
            Assert.False(ts.IsEmpty);
            Assert.Equal(10, ts.Quads.Count());
        }

        [Fact]
        public void LoadFromString_ThrowsExceptionOnInvalidJson()
        {
            var parser = new SafeJsonLdParser();
            var ts = new TripleStore();
            Assert.Throws<JsonReaderException>(() => parser.Load(ts, new StringReader(InvalidJson)));
        }

        [Fact]
        public void LoadFromString_IgnoresInvalidType()
        {
            var parser = new SafeJsonLdParser();
            var ts = new TripleStore();
            parser.Load(ts, new StringReader(InvalidType));
            Assert.False(ts.IsEmpty);
            Assert.Equal(10, ts.Quads.Count());
        }

        [Fact]
        public void LoadFromString_InvokesWarningOnInvalidType()
        {
            var options = new JsonLdNormalizerOptions() { SkipExpansion = true };
            var parser = new SafeJsonLdParser(options);
            var warningRaised = false;
            parser.SafeWarning += m => warningRaised = true;
            var ts = new TripleStore();
            try
            {
                parser.Load(ts, new StringReader(InvalidType));
            }
            catch (DataLossException)
            {
                Assert.False(ts.IsEmpty);
                Assert.Equal(10, ts.Quads.Count());
                Assert.True(warningRaised);
            }
        }
    }
}
