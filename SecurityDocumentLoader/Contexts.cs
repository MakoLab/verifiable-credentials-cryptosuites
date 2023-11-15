using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityDocumentLoader
{
    internal class Contexts
    {
        const string Ed25519Signature2020ContextString = """
            {
              "@context":
              {
                "id": "@id",
                "type": "@type",
                "@protected": true,
                "proof":
                {
                  "@id": "https://w3id.org/security#proof",
                  "@type": "@id",
                  "@container": "@graph"
                },
                "Ed25519VerificationKey2020":
                {
                  "@id": "https://w3id.org/security#Ed25519VerificationKey2020",
                  "@context":
                  {
                    "@protected": true,
                    "id": "@id",
                    "type": "@type",
                    "controller":
                    {
                      "@id": "https://w3id.org/security#controller",
                      "@type": "@id"
                    },
                    "revoked":
                    {
                      "@id": "https://w3id.org/security#revoked",
                      "@type": "http://www.w3.org/2001/XMLSchema#dateTime"
                    },
                    "publicKeyMultibase":
                    {
                      "@id": "https://w3id.org/security#publicKeyMultibase",
                      "@type": "https://w3id.org/security#multibase"
                    }
                  }
                },
                "Ed25519Signature2020":
                {
                  "@id": "https://w3id.org/security#Ed25519Signature2020",
                  "@context":
                  {
                    "@protected": true,
                    "id": "@id",
                    "type": "@type",
                    "challenge": "https://w3id.org/security#challenge",
                    "created":
                    {
                      "@id": "http://purl.org/dc/terms/created",
                      "@type": "http://www.w3.org/2001/XMLSchema#dateTime"
                    },
                    "domain": "https://w3id.org/security#domain",
                    "expires":
                    {
                      "@id": "https://w3id.org/security#expiration",
                      "@type": "http://www.w3.org/2001/XMLSchema#dateTime"
                    },
                    "nonce": "https://w3id.org/security#nonce",
                    "proofPurpose":
                    {
                      "@id": "https://w3id.org/security#proofPurpose",
                      "@type": "@vocab",
                      "@context":
                      {
                        "@protected": true,
                        "id": "@id",
                        "type": "@type",
                        "assertionMethod":
                        {
                          "@id": "https://w3id.org/security#assertionMethod",
                          "@type": "@id",
                          "@container": "@set"
                        },
                        "authentication":
                        {
                          "@id": "https://w3id.org/security#authenticationMethod",
                          "@type": "@id",
                          "@container": "@set"
                        },
                        "capabilityInvocation":
                        {
                          "@id": "https://w3id.org/security#capabilityInvocationMethod",
                          "@type": "@id",
                          "@container": "@set"
                        },
                        "capabilityDelegation":
                        {
                          "@id": "https://w3id.org/security#capabilityDelegationMethod",
                          "@type": "@id",
                          "@container": "@set"
                        },
                        "keyAgreement":
                        {
                          "@id": "https://w3id.org/security#keyAgreementMethod",
                          "@type": "@id",
                          "@container": "@set"
                        }
                      }
                    },
                    "proofValue":
                    {
                      "@id": "https://w3id.org/security#proofValue",
                      "@type": "https://w3id.org/security#multibase"
                    },
                    "verificationMethod":
                    {
                      "@id": "https://w3id.org/security#verificationMethod",
                      "@type": "@id"
                    }
                  }
                }
              }
            }
            """;
        const string VeresOneContextV1String = """
            {
              "@context": {
                "Elector": "https://w3id.org/webledger#Elector",
                "RecoveryElector": "https://w3id.org/webledger#RecoveryElector",
                "ValidatorParameterSet": "https://w3id.org/webledger#ValidatorParameterSet",

                "allowedServiceBaseUrl": {"@id": "https://w3id.org/veres-one#allowedServiceBaseUrl", "@type": "@id", "@container": "@set"},
                "blockHeight": "https://w3id.org/webledger#blockHeight",
                "elector": {"@id": "https://w3id.org/webledger#elector", "@type": "@id"},
                "electorPool": {"@id": "https://w3id.org/webledger#electorPool", "@type": "@id", "@container": "@set"},
                "maximumElectorCount": {"@id": "https://w3id.org/webledger#maximumElectorCount", "@type": "http://www.w3.org/2001/XMLSchema#integer"}
              }
            }
            """;
        const string X25519KeyAgreement2020V1ContextString = """
            {
              "@context": {
                "id": "@id",
                "type": "@type",
                "@protected": true,
                "X25519KeyAgreementKey2020": {
                  "@id": "https://w3id.org/security#X25519KeyAgreementKey2020",
                  "@context": {
                    "@protected": true,
                    "id": "@id",
                    "type": "@type",
                    "controller": {
                      "@id": "https://w3id.org/security#controller",
                      "@type": "@id"
                    },
                    "revoked": {
                      "@id": "https://w3id.org/security#revoked",
                      "@type": "http://www.w3.org/2001/XMLSchema#dateTime"
                    },
                    "publicKeyMultibase": {
                      "@id": "https://w3id.org/security#publicKeyMultibase",
                      "@type": "https://w3id.org/security#multibase"
                    }
                  }
                }
              }
            }
            """;
        const string CredentialsContextV1String = """
            {
              "@context": {
                "@version": 1.1,
                "@protected": true,
                "id": "@id",
                "type": "@type",
                "VerifiableCredential": {
                  "@id": "https://www.w3.org/2018/credentials#VerifiableCredential",
                  "@context": {
                    "@version": 1.1,
                    "@protected": true,
                    "id": "@id",
                    "type": "@type",
                    "cred": "https://www.w3.org/2018/credentials#",
                    "sec": "https://w3id.org/security#",
                    "xsd": "http://www.w3.org/2001/XMLSchema#",
                    "credentialSchema": {
                      "@id": "cred:credentialSchema",
                      "@type": "@id",
                      "@context": {
                        "@version": 1.1,
                        "@protected": true,
                        "id": "@id",
                        "type": "@type",
                        "cred": "https://www.w3.org/2018/credentials#",
                        "JsonSchemaValidator2018": "cred:JsonSchemaValidator2018"
                      }
                    },
                    "credentialStatus": {
                      "@id": "cred:credentialStatus",
                      "@type": "@id"
                    },
                    "credentialSubject": {
                      "@id": "cred:credentialSubject",
                      "@type": "@id"
                    },
                    "evidence": {
                      "@id": "cred:evidence",
                      "@type": "@id"
                    },
                    "expirationDate": {
                      "@id": "cred:expirationDate",
                      "@type": "xsd:dateTime"
                    },
                    "holder": {
                      "@id": "cred:holder",
                      "@type": "@id"
                    },
                    "issued": {
                      "@id": "cred:issued",
                      "@type": "xsd:dateTime"
                    },
                    "issuer": {
                      "@id": "cred:issuer",
                      "@type": "@id"
                    },
                    "issuanceDate": {
                      "@id": "cred:issuanceDate",
                      "@type": "xsd:dateTime"
                    },
                    "proof": {
                      "@id": "sec:proof",
                      "@type": "@id",
                      "@container": "@graph"
                    },
                    "refreshService": {
                      "@id": "cred:refreshService",
                      "@type": "@id",
                      "@context": {
                        "@version": 1.1,
                        "@protected": true,
                        "id": "@id",
                        "type": "@type",
                        "cred": "https://www.w3.org/2018/credentials#",
                        "ManualRefreshService2018": "cred:ManualRefreshService2018"
                      }
                    },
                    "termsOfUse": {
                      "@id": "cred:termsOfUse",
                      "@type": "@id"
                    },
                    "validFrom": {
                      "@id": "cred:validFrom",
                      "@type": "xsd:dateTime"
                    },
                    "validUntil": {
                      "@id": "cred:validUntil",
                      "@type": "xsd:dateTime"
                    }
                  }
                },
                "VerifiablePresentation": {
                  "@id": "https://www.w3.org/2018/credentials#VerifiablePresentation",
                  "@context": {
                    "@version": 1.1,
                    "@protected": true,
                    "id": "@id",
                    "type": "@type",
                    "cred": "https://www.w3.org/2018/credentials#",
                    "sec": "https://w3id.org/security#",
                    "holder": {
                      "@id": "cred:holder",
                      "@type": "@id"
                    },
                    "proof": {
                      "@id": "sec:proof",
                      "@type": "@id",
                      "@container": "@graph"
                    },
                    "verifiableCredential": {
                      "@id": "cred:verifiableCredential",
                      "@type": "@id",
                      "@container": "@graph"
                    }
                  }
                },
                "EcdsaSecp256k1Signature2019": {
                  "@id": "https://w3id.org/security#EcdsaSecp256k1Signature2019",
                  "@context": {
                    "@version": 1.1,
                    "@protected": true,
                    "id": "@id",
                    "type": "@type",
                    "sec": "https://w3id.org/security#",
                    "xsd": "http://www.w3.org/2001/XMLSchema#",
                    "challenge": "sec:challenge",
                    "created": {
                      "@id": "http://purl.org/dc/terms/created",
                      "@type": "xsd:dateTime"
                    },
                    "domain": "sec:domain",
                    "expires": {
                      "@id": "sec:expiration",
                      "@type": "xsd:dateTime"
                    },
                    "jws": "sec:jws",
                    "nonce": "sec:nonce",
                    "proofPurpose": {
                      "@id": "sec:proofPurpose",
                      "@type": "@vocab",
                      "@context": {
                        "@version": 1.1,
                        "@protected": true,
                        "id": "@id",
                        "type": "@type",
                        "sec": "https://w3id.org/security#",
                        "assertionMethod": {
                          "@id": "sec:assertionMethod",
                          "@type": "@id",
                          "@container": "@set"
                        },
                        "authentication": {
                          "@id": "sec:authenticationMethod",
                          "@type": "@id",
                          "@container": "@set"
                        }
                      }
                    },
                    "proofValue": "sec:proofValue",
                    "verificationMethod": {
                      "@id": "sec:verificationMethod",
                      "@type": "@id"
                    }
                  }
                },
                "EcdsaSecp256r1Signature2019": {
                  "@id": "https://w3id.org/security#EcdsaSecp256r1Signature2019",
                  "@context": {
                    "@version": 1.1,
                    "@protected": true,
                    "id": "@id",
                    "type": "@type",
                    "sec": "https://w3id.org/security#",
                    "xsd": "http://www.w3.org/2001/XMLSchema#",
                    "challenge": "sec:challenge",
                    "created": {
                      "@id": "http://purl.org/dc/terms/created",
                      "@type": "xsd:dateTime"
                    },
                    "domain": "sec:domain",
                    "expires": {
                      "@id": "sec:expiration",
                      "@type": "xsd:dateTime"
                    },
                    "jws": "sec:jws",
                    "nonce": "sec:nonce",
                    "proofPurpose": {
                      "@id": "sec:proofPurpose",
                      "@type": "@vocab",
                      "@context": {
                        "@version": 1.1,
                        "@protected": true,
                        "id": "@id",
                        "type": "@type",
                        "sec": "https://w3id.org/security#",
                        "assertionMethod": {
                          "@id": "sec:assertionMethod",
                          "@type": "@id",
                          "@container": "@set"
                        },
                        "authentication": {
                          "@id": "sec:authenticationMethod",
                          "@type": "@id",
                          "@container": "@set"
                        }
                      }
                    },
                    "proofValue": "sec:proofValue",
                    "verificationMethod": {
                      "@id": "sec:verificationMethod",
                      "@type": "@id"
                    }
                  }
                },
                "Ed25519Signature2018": {
                  "@id": "https://w3id.org/security#Ed25519Signature2018",
                  "@context": {
                    "@version": 1.1,
                    "@protected": true,
                    "id": "@id",
                    "type": "@type",
                    "sec": "https://w3id.org/security#",
                    "xsd": "http://www.w3.org/2001/XMLSchema#",
                    "challenge": "sec:challenge",
                    "created": {
                      "@id": "http://purl.org/dc/terms/created",
                      "@type": "xsd:dateTime"
                    },
                    "domain": "sec:domain",
                    "expires": {
                      "@id": "sec:expiration",
                      "@type": "xsd:dateTime"
                    },
                    "jws": "sec:jws",
                    "nonce": "sec:nonce",
                    "proofPurpose": {
                      "@id": "sec:proofPurpose",
                      "@type": "@vocab",
                      "@context": {
                        "@version": 1.1,
                        "@protected": true,
                        "id": "@id",
                        "type": "@type",
                        "sec": "https://w3id.org/security#",
                        "assertionMethod": {
                          "@id": "sec:assertionMethod",
                          "@type": "@id",
                          "@container": "@set"
                        },
                        "authentication": {
                          "@id": "sec:authenticationMethod",
                          "@type": "@id",
                          "@container": "@set"
                        }
                      }
                    },
                    "proofValue": "sec:proofValue",
                    "verificationMethod": {
                      "@id": "sec:verificationMethod",
                      "@type": "@id"
                    }
                  }
                },
                "RsaSignature2018": {
                  "@id": "https://w3id.org/security#RsaSignature2018",
                  "@context": {
                    "@version": 1.1,
                    "@protected": true,
                    "challenge": "sec:challenge",
                    "created": {
                      "@id": "http://purl.org/dc/terms/created",
                      "@type": "xsd:dateTime"
                    },
                    "domain": "sec:domain",
                    "expires": {
                      "@id": "sec:expiration",
                      "@type": "xsd:dateTime"
                    },
                    "jws": "sec:jws",
                    "nonce": "sec:nonce",
                    "proofPurpose": {
                      "@id": "sec:proofPurpose",
                      "@type": "@vocab",
                      "@context": {
                        "@version": 1.1,
                        "@protected": true,
                        "id": "@id",
                        "type": "@type",
                        "sec": "https://w3id.org/security#",
                        "assertionMethod": {
                          "@id": "sec:assertionMethod",
                          "@type": "@id",
                          "@container": "@set"
                        },
                        "authentication": {
                          "@id": "sec:authenticationMethod",
                          "@type": "@id",
                          "@container": "@set"
                        }
                      }
                    },
                    "proofValue": "sec:proofValue",
                    "verificationMethod": {
                      "@id": "sec:verificationMethod",
                      "@type": "@id"
                    }
                  }
                },
                "proof": {
                  "@id": "https://w3id.org/security#proof",
                  "@type": "@id",
                  "@container": "@graph"
                }
              }
            }
            """;
        const string DidContextV1String = """
            {
              "@context": {
                "@protected": true,
                "id": "@id",
                "type": "@type",
                "alsoKnownAs": {
                  "@id": "https://www.w3.org/ns/activitystreams#alsoKnownAs",
                  "@type": "@id"
                },
                "assertionMethod": {
                  "@id": "https://w3id.org/security#assertionMethod",
                  "@type": "@id",
                  "@container": "@set"
                },
                "authentication": {
                  "@id": "https://w3id.org/security#authenticationMethod",
                  "@type": "@id",
                  "@container": "@set"
                },
                "capabilityDelegation": {
                  "@id": "https://w3id.org/security#capabilityDelegationMethod",
                  "@type": "@id",
                  "@container": "@set"
                },
                "capabilityInvocation": {
                  "@id": "https://w3id.org/security#capabilityInvocationMethod",
                  "@type": "@id",
                  "@container": "@set"
                },
                "controller": {
                  "@id": "https://w3id.org/security#controller",
                  "@type": "@id"
                },
                "keyAgreement": {
                  "@id": "https://w3id.org/security#keyAgreementMethod",
                  "@type": "@id",
                  "@container": "@set"
                },
                "service": {
                  "@id": "https://www.w3.org/ns/did#service",
                  "@type": "@id",
                  "@context": {
                    "@protected": true,
                    "id": "@id",
                    "type": "@type",
                    "serviceEndpoint": {
                      "@id": "https://www.w3.org/ns/did#serviceEndpoint",
                      "@type": "@id"
                    }
                  }
                },
                "verificationMethod": {
                  "@id": "https://w3id.org/security#verificationMethod",
                  "@type": "@id"
                }
              }
            }
            """;
        const string DataIntegrityV1String = """
            {
              "@context": {
                "id": "@id",
                "type": "@type",
                "@protected": true,
                "proof": {
                  "@id": "https://w3id.org/security#proof",
                  "@type": "@id",
                  "@container": "@graph"
                },
                "DataIntegrityProof": {
                  "@id": "https://w3id.org/security#DataIntegrityProof",
                  "@context": {
                    "@protected": true,
                    "id": "@id",
                    "type": "@type",
                    "challenge": "https://w3id.org/security#challenge",
                    "created": {
                      "@id": "http://purl.org/dc/terms/created",
                      "@type": "http://www.w3.org/2001/XMLSchema#dateTime"
                    },
                    "domain": "https://w3id.org/security#domain",
                    "expires": {
                      "@id": "https://w3id.org/security#expiration",
                      "@type": "http://www.w3.org/2001/XMLSchema#dateTime"
                    },
                    "nonce": "https://w3id.org/security#nonce",
                    "proofPurpose": {
                      "@id": "https://w3id.org/security#proofPurpose",
                      "@type": "@vocab",
                      "@context": {
                        "@protected": true,
                        "id": "@id",
                        "type": "@type",
                        "assertionMethod": {
                          "@id": "https://w3id.org/security#assertionMethod",
                          "@type": "@id",
                          "@container": "@set"
                        },
                        "authentication": {
                          "@id": "https://w3id.org/security#authenticationMethod",
                          "@type": "@id",
                          "@container": "@set"
                        },
                        "capabilityInvocation": {
                          "@id": "https://w3id.org/security#capabilityInvocationMethod",
                          "@type": "@id",
                          "@container": "@set"
                        },
                        "capabilityDelegation": {
                          "@id": "https://w3id.org/security#capabilityDelegationMethod",
                          "@type": "@id",
                          "@container": "@set"
                        },
                        "keyAgreement": {
                          "@id": "https://w3id.org/security#keyAgreementMethod",
                          "@type": "@id",
                          "@container": "@set"
                        }
                      }
                    },
                    "cryptosuite": "https://w3id.org/security#cryptosuite",
                    "proofValue": {
                      "@id": "https://w3id.org/security#proofValue",
                      "@type": "https://w3id.org/security#multibase"
                    },
                    "verificationMethod": {
                      "@id": "https://w3id.org/security#verificationMethod",
                      "@type": "@id"
                    }
                  }
                }
              }
            }
            """;

        internal static readonly JToken VeresOneContextV1 = JToken.Parse(VeresOneContextV1String);
        internal static readonly JToken Ed25519Signature2020Context = JToken.Parse(Ed25519Signature2020ContextString);
        internal static readonly JToken X25519KeyAgreement2020V1Context = JToken.Parse(X25519KeyAgreement2020V1ContextString);
        internal static readonly JToken CredentialsContextV1 = JToken.Parse(CredentialsContextV1String);
        internal static readonly JToken DidContextV1 = JToken.Parse(DidContextV1String);
        internal static readonly JToken DataIntegrityV1 = JToken.Parse(DataIntegrityV1String);
    }
}
