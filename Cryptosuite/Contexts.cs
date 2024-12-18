﻿using Newtonsoft.Json.Linq;

namespace Cryptosuite.Core
{
    public class Contexts
    {
        public const string Ed25519Signature2020ContextUrl = "https://w3id.org/security/suites/ed25519-2020/v1";
        public const string VeresOneContextV1Url = "https://w3id.org/veres-one/v1";
        public const string X25519KeyAgreement2020V1ContextUrl = "https://w3id.org/security/suites/x25519-2020/v1";
        public const string CredentialsContextV1Url = "https://www.w3.org/2018/credentials/v1";
        public const string CredentialsContextV2Url = "https://www.w3.org/ns/credentials/v2";
        public const string DidContextV1Url = "https://www.w3.org/ns/did/v1";
        public const string DataIntegrityV1Url = "https://w3id.org/security/data-integrity/v1";
        public const string DataIntegrityV2Url = "https://w3id.org/security/data-integrity/v2";
        public const string SecurityContextV1Url = "https://w3id.org/security/v1";
        public const string SecurityContextV2Url = "https://w3id.org/security/v2";
        public const string SecurityContextUrl = "https://w3id.org/security/v2";
        public const string MultikeyContextV1Url = "https://w3id.org/security/multikey/v1";

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
        const string CredentialsContextV2String = """
            {
                "@context": {
                    "@protected": true,
                    "id": "@id",
                    "type": "@type",
                    "description": "https://schema.org/description",
                    "digestMultibase": {
                        "@id": "https://w3id.org/security#digestMultibase",
                        "@type": "https://w3id.org/security#multibase"
                    },
                    "digestSRI": {
                        "@id": "https://www.w3.org/2018/credentials#digestSRI",
                        "@type": "https://www.w3.org/2018/credentials#sriString"
                    },
                    "mediaType": {
                        "@id": "https://schema.org/encodingFormat"
                    },
                    "name": "https://schema.org/name",
                    "VerifiableCredential": {
                        "@id": "https://www.w3.org/2018/credentials#VerifiableCredential",
                        "@context": {
                            "@protected": true,
                            "id": "@id",
                            "type": "@type",
                            "confidenceMethod": {
                                "@id": "https://www.w3.org/2018/credentials#confidenceMethod",
                                "@type": "@id"
                            },
                            "credentialSchema": {
                                "@id": "https://www.w3.org/2018/credentials#credentialSchema",
                                "@type": "@id"
                            },
                            "credentialStatus": {
                                "@id": "https://www.w3.org/2018/credentials#credentialStatus",
                                "@type": "@id"
                            },
                            "credentialSubject": {
                                "@id": "https://www.w3.org/2018/credentials#credentialSubject",
                                "@type": "@id"
                            },
                            "description": "https://schema.org/description",
                            "evidence": {
                                "@id": "https://www.w3.org/2018/credentials#evidence",
                                "@type": "@id"
                            },
                            "issuer": {
                                "@id": "https://www.w3.org/2018/credentials#issuer",
                                "@type": "@id"
                            },
                            "name": "https://schema.org/name",
                            "proof": {
                                "@id": "https://w3id.org/security#proof",
                                "@type": "@id",
                                "@container": "@graph"
                            },
                            "refreshService": {
                                "@id": "https://www.w3.org/2018/credentials#refreshService",
                                "@type": "@id"
                            },
                            "relatedResource": {
                                "@id": "https://www.w3.org/2018/credentials#relatedResource",
                                "@type": "@id"
                            },
                            "renderMethod": {
                                "@id": "https://www.w3.org/2018/credentials#renderMethod",
                                "@type": "@id"
                            },
                            "termsOfUse": {
                                "@id": "https://www.w3.org/2018/credentials#termsOfUse",
                                "@type": "@id"
                            },
                            "validFrom": {
                                "@id": "https://www.w3.org/2018/credentials#validFrom",
                                "@type": "http://www.w3.org/2001/XMLSchema#dateTime"
                            },
                            "validUntil": {
                                "@id": "https://www.w3.org/2018/credentials#validUntil",
                                "@type": "http://www.w3.org/2001/XMLSchema#dateTime"
                            }
                        }
                    },
                    "EnvelopedVerifiableCredential": "https://www.w3.org/2018/credentials#EnvelopedVerifiableCredential",
                    "VerifiablePresentation": {
                        "@id": "https://www.w3.org/2018/credentials#VerifiablePresentation",
                        "@context": {
                            "@protected": true,
                            "id": "@id",
                            "type": "@type",
                            "holder": {
                                "@id": "https://www.w3.org/2018/credentials#holder",
                                "@type": "@id"
                            },
                            "proof": {
                                "@id": "https://w3id.org/security#proof",
                                "@type": "@id",
                                "@container": "@graph"
                            },
                            "termsOfUse": {
                                "@id": "https://www.w3.org/2018/credentials#termsOfUse",
                                "@type": "@id"
                            },
                            "verifiableCredential": {
                                "@id": "https://www.w3.org/2018/credentials#verifiableCredential",
                                "@type": "@id",
                                "@container": "@graph",
                                "@context": null
                            }
                        }
                    },
                    "EnvelopedVerifiablePresentation": "https://www.w3.org/2018/credentials#EnvelopedVerifiablePresentation",
                    "JsonSchemaCredential": "https://www.w3.org/2018/credentials#JsonSchemaCredential",
                    "JsonSchema": {
                        "@id": "https://www.w3.org/2018/credentials#JsonSchema",
                        "@context": {
                            "@protected": true,
                            "id": "@id",
                            "type": "@type",
                            "jsonSchema": {
                                "@id": "https://www.w3.org/2018/credentials#jsonSchema",
                                "@type": "@json"
                            }
                        }
                    },
                    "BitstringStatusListCredential": "https://www.w3.org/ns/credentials/status#BitstringStatusListCredential",
                    "BitstringStatusList": {
                        "@id": "https://www.w3.org/ns/credentials/status#BitstringStatusList",
                        "@context": {
                            "@protected": true,
                            "id": "@id",
                            "type": "@type",
                            "encodedList": {
                                "@id": "https://www.w3.org/ns/credentials/status#encodedList",
                                "@type": "https://w3id.org/security#multibase"
                            },
                            "statusMessage": {
                                "@id": "https://www.w3.org/ns/credentials/status#statusMessage",
                                "@context": {
                                    "@protected": true,
                                    "id": "@id",
                                    "type": "@type",
                                    "message": "https://www.w3.org/ns/credentials/status#message",
                                    "status": "https://www.w3.org/ns/credentials/status#status"
                                }
                            },
                            "statusPurpose": "https://www.w3.org/ns/credentials/status#statusPurpose",
                            "statusReference": {
                                "@id": "https://www.w3.org/ns/credentials/status#statusReference",
                                "@type": "@id"
                            },
                            "statusSize": {
                                "@id": "https://www.w3.org/ns/credentials/status#statusSize",
                                "@type": "https://www.w3.org/2001/XMLSchema#positiveInteger"
                            },
                            "ttl": "https://www.w3.org/ns/credentials/status#ttl"
                        }
                    },
                    "BitstringStatusListEntry": {
                        "@id": "https://www.w3.org/ns/credentials/status#BitstringStatusListEntry",
                        "@context": {
                            "@protected": true,
                            "id": "@id",
                            "type": "@type",
                            "statusListCredential": {
                                "@id": "https://www.w3.org/ns/credentials/status#statusListCredential",
                                "@type": "@id"
                            },
                            "statusListIndex": "https://www.w3.org/ns/credentials/status#statusListIndex",
                            "statusPurpose": "https://www.w3.org/ns/credentials/status#statusPurpose"
                        }
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
                            "cryptosuite": {
                                "@id": "https://w3id.org/security#cryptosuite",
                                "@type": "https://w3id.org/security#cryptosuiteString"
                            },
                            "domain": "https://w3id.org/security#domain",
                            "expires": {
                                "@id": "https://w3id.org/security#expiration",
                                "@type": "http://www.w3.org/2001/XMLSchema#dateTime"
                            },
                            "nonce": "https://w3id.org/security#nonce",
                            "previousProof": {
                                "@id": "https://w3id.org/security#previousProof",
                                "@type": "@id"
                            },
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
                                    "keyAgreement": {
                                        "@id": "https://w3id.org/security#keyAgreementMethod",
                                        "@type": "@id",
                                        "@container": "@set"
                                    }
                                }
                            },
                            "proofValue": {
                                "@id": "https://w3id.org/security#proofValue",
                                "@type": "https://w3id.org/security#multibase"
                            },
                            "verificationMethod": {
                                "@id": "https://w3id.org/security#verificationMethod",
                                "@type": "@id"
                            }
                        }
                    },
                    "...": {
                        "@id": "https://www.iana.org/assignments/jwt#..."
                    },
                    "_sd": {
                        "@id": "https://www.iana.org/assignments/jwt#_sd",
                        "@type": "@json"
                    },
                    "_sd_alg": {
                        "@id": "https://www.iana.org/assignments/jwt#_sd_alg"
                    },
                    "aud": {
                        "@id": "https://www.iana.org/assignments/jwt#aud",
                        "@type": "@id"
                    },
                    "cnf": {
                        "@id": "https://www.iana.org/assignments/jwt#cnf",
                        "@context": {
                            "@protected": true,
                            "kid": {
                                "@id": "https://www.iana.org/assignments/jwt#kid",
                                "@type": "@id"
                            },
                            "jwk": {
                                "@id": "https://www.iana.org/assignments/jwt#jwk",
                                "@type": "@json"
                            }
                        }
                    },
                    "exp": {
                        "@id": "https://www.iana.org/assignments/jwt#exp",
                        "@type": "https://www.w3.org/2001/XMLSchema#nonNegativeInteger"
                    },
                    "iat": {
                        "@id": "https://www.iana.org/assignments/jwt#iat",
                        "@type": "https://www.w3.org/2001/XMLSchema#nonNegativeInteger"
                    },
                    "iss": {
                        "@id": "https://www.iana.org/assignments/jose#iss",
                        "@type": "@id"
                    },
                    "jku": {
                        "@id": "https://www.iana.org/assignments/jose#jku",
                        "@type": "@id"
                    },
                    "kid": {
                        "@id": "https://www.iana.org/assignments/jose#kid",
                        "@type": "@id"
                    },
                    "nbf": {
                        "@id": "https://www.iana.org/assignments/jwt#nbf",
                        "@type": "https://www.w3.org/2001/XMLSchema#nonNegativeInteger"
                    },
                    "sub": {
                        "@id": "https://www.iana.org/assignments/jose#sub",
                        "@type": "@id"
                    },
                    "x5u": {
                        "@id": "https://www.iana.org/assignments/jose#x5u",
                        "@type": "@id"
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
        const string DataIntegrityV2String = """
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
                    "previousProof": {
                      "@id": "https://w3id.org/security#previousProof",
                      "@type": "@id"
                    },
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
                    "cryptosuite": {
                      "@id": "https://w3id.org/security#cryptosuite",
                      "@type": "https://w3id.org/security#cryptosuiteString"
                    },
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
        const string SecurityContextV1String = """
            {
              "@context": {
                "id": "@id",
                "type": "@type",
                "dc": "http://purl.org/dc/terms/",
                "sec": "https://w3id.org/security#",
                "xsd": "http://www.w3.org/2001/XMLSchema#",
                "EcdsaKoblitzSignature2016": "sec:EcdsaKoblitzSignature2016",
                "Ed25519Signature2018": "sec:Ed25519Signature2018",
                "EncryptedMessage": "sec:EncryptedMessage",
                "GraphSignature2012": "sec:GraphSignature2012",
                "LinkedDataSignature2015": "sec:LinkedDataSignature2015",
                "LinkedDataSignature2016": "sec:LinkedDataSignature2016",
                "CryptographicKey": "sec:Key",
                "authenticationTag": "sec:authenticationTag",
                "canonicalizationAlgorithm": "sec:canonicalizationAlgorithm",
                "cipherAlgorithm": "sec:cipherAlgorithm",
                "cipherData": "sec:cipherData",
                "cipherKey": "sec:cipherKey",
                "created": {
                  "@id": "dc:created",
                  "@type": "xsd:dateTime"
                },
                "creator": {
                  "@id": "dc:creator",
                  "@type": "@id"
                },
                "digestAlgorithm": "sec:digestAlgorithm",
                "digestValue": "sec:digestValue",
                "domain": "sec:domain",
                "encryptionKey": "sec:encryptionKey",
                "expiration": {
                  "@id": "sec:expiration",
                  "@type": "xsd:dateTime"
                },
                "expires": {
                  "@id": "sec:expiration",
                  "@type": "xsd:dateTime"
                },
                "initializationVector": "sec:initializationVector",
                "iterationCount": "sec:iterationCount",
                "nonce": "sec:nonce",
                "normalizationAlgorithm": "sec:normalizationAlgorithm",
                "owner": {
                  "@id": "sec:owner",
                  "@type": "@id"
                },
                "password": "sec:password",
                "privateKey": {
                  "@id": "sec:privateKey",
                  "@type": "@id"
                },
                "privateKeyPem": "sec:privateKeyPem",
                "publicKey": {
                  "@id": "sec:publicKey",
                  "@type": "@id"
                },
                "publicKeyBase58": "sec:publicKeyBase58",
                "publicKeyPem": "sec:publicKeyPem",
                "publicKeyWif": "sec:publicKeyWif",
                "publicKeyService": {
                  "@id": "sec:publicKeyService",
                  "@type": "@id"
                },
                "revoked": {
                  "@id": "sec:revoked",
                  "@type": "xsd:dateTime"
                },
                "salt": "sec:salt",
                "signature": "sec:signature",
                "signatureAlgorithm": "sec:signingAlgorithm",
                "signatureValue": "sec:signatureValue"
              }
            }
            """;
        const string SecurityContextV2String = """
            {
              "@context": [
                {
                  "@version": 1.1
                },
                "https://w3id.org/security/v1",
                {
                  "AesKeyWrappingKey2019": "sec:AesKeyWrappingKey2019",
                  "DeleteKeyOperation": "sec:DeleteKeyOperation",
                  "DeriveSecretOperation": "sec:DeriveSecretOperation",
                  "EcdsaSecp256k1Signature2019": "sec:EcdsaSecp256k1Signature2019",
                  "EcdsaSecp256r1Signature2019": "sec:EcdsaSecp256r1Signature2019",
                  "EcdsaSecp256k1VerificationKey2019": "sec:EcdsaSecp256k1VerificationKey2019",
                  "EcdsaSecp256r1VerificationKey2019": "sec:EcdsaSecp256r1VerificationKey2019",
                  "Ed25519Signature2018": "sec:Ed25519Signature2018",
                  "Ed25519VerificationKey2018": "sec:Ed25519VerificationKey2018",
                  "EquihashProof2018": "sec:EquihashProof2018",
                  "ExportKeyOperation": "sec:ExportKeyOperation",
                  "GenerateKeyOperation": "sec:GenerateKeyOperation",
                  "KmsOperation": "sec:KmsOperation",
                  "RevokeKeyOperation": "sec:RevokeKeyOperation",
                  "RsaSignature2018": "sec:RsaSignature2018",
                  "RsaVerificationKey2018": "sec:RsaVerificationKey2018",
                  "Sha256HmacKey2019": "sec:Sha256HmacKey2019",
                  "SignOperation": "sec:SignOperation",
                  "UnwrapKeyOperation": "sec:UnwrapKeyOperation",
                  "VerifyOperation": "sec:VerifyOperation",
                  "WrapKeyOperation": "sec:WrapKeyOperation",
                  "X25519KeyAgreementKey2019": "sec:X25519KeyAgreementKey2019",
                  "allowedAction": "sec:allowedAction",
                  "assertionMethod": {
                    "@id": "sec:assertionMethod",
                    "@type": "@id",
                    "@container": "@set"
                  },
                  "authentication": {
                    "@id": "sec:authenticationMethod",
                    "@type": "@id",
                    "@container": "@set"
                  },
                  "capability": {
                    "@id": "sec:capability",
                    "@type": "@id"
                  },
                  "capabilityAction": "sec:capabilityAction",
                  "capabilityChain": {
                    "@id": "sec:capabilityChain",
                    "@type": "@id",
                    "@container": "@list"
                  },
                  "capabilityDelegation": {
                    "@id": "sec:capabilityDelegationMethod",
                    "@type": "@id",
                    "@container": "@set"
                  },
                  "capabilityInvocation": {
                    "@id": "sec:capabilityInvocationMethod",
                    "@type": "@id",
                    "@container": "@set"
                  },
                  "caveat": {
                    "@id": "sec:caveat",
                    "@type": "@id",
                    "@container": "@set"
                  },
                  "challenge": "sec:challenge",
                  "ciphertext": "sec:ciphertext",
                  "controller": {
                    "@id": "sec:controller",
                    "@type": "@id"
                  },
                  "delegator": {
                    "@id": "sec:delegator",
                    "@type": "@id"
                  },
                  "equihashParameterK": {
                    "@id": "sec:equihashParameterK",
                    "@type": "xsd:integer"
                  },
                  "equihashParameterN": {
                    "@id": "sec:equihashParameterN",
                    "@type": "xsd:integer"
                  },
                  "invocationTarget": {
                    "@id": "sec:invocationTarget",
                    "@type": "@id"
                  },
                  "invoker": {
                    "@id": "sec:invoker",
                    "@type": "@id"
                  },
                  "jws": "sec:jws",
                  "keyAgreement": {
                    "@id": "sec:keyAgreementMethod",
                    "@type": "@id",
                    "@container": "@set"
                  },
                  "kmsModule": {
                    "@id": "sec:kmsModule"
                  },
                  "parentCapability": {
                    "@id": "sec:parentCapability",
                    "@type": "@id"
                  },
                  "plaintext": "sec:plaintext",
                  "proof": {
                    "@id": "sec:proof",
                    "@type": "@id",
                    "@container": "@graph"
                  },
                  "proofPurpose": {
                    "@id": "sec:proofPurpose",
                    "@type": "@vocab"
                  },
                  "proofValue": "sec:proofValue",
                  "referenceId": "sec:referenceId",
                  "unwrappedKey": "sec:unwrappedKey",
                  "verificationMethod": {
                    "@id": "sec:verificationMethod",
                    "@type": "@id"
                  },
                  "verifyData": "sec:verifyData",
                  "wrappedKey": "sec:wrappedKey"
                }
              ]
            }
            """;
        const string MultikeyContextV1String = """
            {
                "@context": {
                    "id": "@id",
                    "type": "@type",
                    "@protected": true,
                    "Multikey": {
                        "@id": "https://w3id.org/security#Multikey",
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
                            "expires": {
                                "@id": "https://w3id.org/security#expiration",
                                "@type": "http://www.w3.org/2001/XMLSchema#dateTime"
                            },
                            "publicKeyMultibase": {
                                "@id": "https://w3id.org/security#publicKeyMultibase",
                                "@type": "https://w3id.org/security#multibase"
                            },
                            "secretKeyMultibase": {
                                "@id": "https://w3id.org/security#secretKeyMultibase",
                                "@type": "https://w3id.org/security#multibase"
                            }
                        }
                    }
                }
            }
            """;

        private static JToken? _veresOneContextV1;
        private static JToken? _ed25519Signature2020Context;
        private static JToken? _x25519KeyAgreement2020V1Context;
        private static JToken? _credentialsContextV1;
        private static JToken? _credentialsContextV2;
        private static JToken? _didContextV1;
        private static JToken? _dataIntegrityV1;
        private static JToken? _dataIntegrityV2;
        private static JToken? _securityContextV1;
        private static JToken? _securityContextV2;
        private static JToken? _multikeyContextV1;

        public static JToken VeresOneContextV1
        {
            get
            {
                _veresOneContextV1 ??= JToken.Parse(VeresOneContextV1String);
                return _veresOneContextV1;
            }
        }

        public static JToken Ed25519Signature2020Context
        {
            get
            {
                _ed25519Signature2020Context ??= JToken.Parse(Ed25519Signature2020ContextString);
                return _ed25519Signature2020Context;
            }
        }

        public static JToken X25519KeyAgreement2020V1Context
        {
            get
            {
                _x25519KeyAgreement2020V1Context ??= JToken.Parse(X25519KeyAgreement2020V1ContextString);
                return _x25519KeyAgreement2020V1Context;
            }
        }

        public static JToken CredentialsContextV1
        {
            get
            {
                _credentialsContextV1 ??= JToken.Parse(CredentialsContextV1String);
                return _credentialsContextV1;
            }
        }

        public static JToken CredentialsContextV2
        {
            get
            {
                _credentialsContextV2 ??= JToken.Parse(CredentialsContextV2String);
                return _credentialsContextV2;
            }
        }

        public static JToken DidContextV1
        {
            get
            {
                _didContextV1 ??= JToken.Parse(DidContextV1String);
                return _didContextV1;
            }
        }

        public static JToken DataIntegrityV1
        {
            get
            {
                _dataIntegrityV1 ??= JToken.Parse(DataIntegrityV1String);
                return _dataIntegrityV1;
            }
        }

        public static JToken DataIntegrityV2
        {
            get
            {
                _dataIntegrityV2 ??= JToken.Parse(DataIntegrityV2String);
                return _dataIntegrityV2;
            }
        }

        public static JToken SecurityContextV1
        {
            get
            {
                _securityContextV1 ??= JToken.Parse(SecurityContextV1String);
                return _securityContextV1;
            }
        }

        public static JToken SecurityContextV2
        {
            get
            {
                _securityContextV2 ??= JToken.Parse(SecurityContextV2String);
                return _securityContextV2;
            }
        }

        public static JToken MultikeyContextV1
        {
            get
            {
                _multikeyContextV1 ??= JToken.Parse(MultikeyContextV1String);
                return _multikeyContextV1;
            }
        }
    }
}
