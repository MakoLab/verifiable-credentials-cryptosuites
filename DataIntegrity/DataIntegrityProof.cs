﻿using Cryptosuite.Core;
using Cryptosuite.Core.ControllerDocuments;
using Cryptosuite.Core.Interfaces;
using Cryptosuite.Core.Util;
using ECDsa_Multikey;
using FluentResults;
using JsonLdExtensions;
using JsonLdExtensions.Canonicalization;
using JsonLdSignatures.Purposes;
using JsonLdSignatures.Suites;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using VDS.RDF.JsonLd;

namespace DataIntegrity
{
    public class DataIntegrityProof : LinkedDataSignature
    {
        private const string ProofType = "DataIntegrityProof";
        private const string DataIntegrityContext = Contexts.DataIntegrityV2Url;
        private const string VC20Context = Contexts.CredentialsContextV2Url;

        private readonly string MultiBaseBase58BtcHeader = Constants.MultibaseBase58Header.ToString();
        private readonly string MultiBaseBase64UrlHeader = Constants.MultibaseBase64Header.ToString();
        private readonly ICryptosuite _cryptoSuite;
        private readonly Signer? _signer;
        private readonly DateTime? _date;
        private readonly string? _verificationMethod;
        private Proof? _proof;
        private byte[]? _hashCache;
        private JObject? _hashDocument;
        private readonly JsonSerializer _serializer;
        private static readonly SHA256 SHA256 = SHA256.Create();

        public string ContextUrl { get; set; } = DataIntegrityContext;

        public DataIntegrityProof(ICryptosuite cryptoSuite, Signer? signer = null, DateTime? date = null) : base(ProofType)
        {
            _cryptoSuite = cryptoSuite;
            _date = date;
            (_signer, _verificationMethod) = ProcessSignatureParams(signer, _cryptoSuite.RequiredAlgorithm);
            _serializer = new JsonSerializer();
            _serializer.Converters.Add(new SingleArrayConverter<string>());
        }

        public override Proof CreateProof(JObject document, ProofPurpose purpose, IEnumerable<Proof> proofSet, IDocumentLoader documentLoader)
        {
            var proof = _proof is null ? new Proof() { Type = "DataIntegrityProof" } : new Proof(_proof);
            proof.Type = Type;
            var date = _date;
            if (proof.Created is null && date is null)
            {
                date = DateTime.UtcNow;
            }
            if (date is not null)
            {
                proof.Created = date;
            }
            proof.VerificationMethod = _verificationMethod;
            proof.CryptoSuiteName = _cryptoSuite.Name;
            proof = UpdateProof(document, proof, purpose, proofSet, documentLoader);
            proof = purpose.Update(proof);
            byte[] verifyData;
            if (_cryptoSuite is ICreateVerifyData csv)
            {
                verifyData = csv.CreateVerifyData(document, proof, proofSet, documentLoader);
            }
            else
            {
                verifyData = CreateVerifyData(document, proof, documentLoader);
            }
            Debug.WriteLine($"Verify data: {Convert.ToHexString(verifyData).ToLower()}");
            if (_signer is null)
            {
                throw new InvalidOperationException("The signer is not defined.");
            }
            if (_cryptoSuite is ICreateProofValue csp)
            {
                proof.ProofValue = csp.CreateProofValue(verifyData, document, proof, proofSet, documentLoader, _signer);
            }
            else
            {
                proof = Sign(verifyData, proof);
            }
            return proof;
        }

        public override JObject Derive(JObject document, ProofPurpose purpose, IEnumerable<Proof> proofSet, IDocumentLoader documentLoader)
        {
            if (_cryptoSuite is IDerive cs)
            {
                return cs.Derive(document, purpose, proofSet, documentLoader);
            }
            else
            {
                throw new Exception($"The cryptosuite {_cryptoSuite.Name} does not support derivation.");
            }
        }

        public override Result<VerificationMethod> VerifyProof(
            Proof proof, JObject document, ProofPurpose purpose, IEnumerable<Proof> proofSet,
            IDocumentLoader documentLoader)
        {
            byte[] verifyData;
            if (_cryptoSuite is ICreateVerifyData csv)
            {
                verifyData = csv.CreateVerifyData(document, proof, proofSet, documentLoader);
            }
            else
            {
                verifyData = CreateVerifyData(document, proof, documentLoader);
            }
            var verificationMethod = GetVerificationMethod(proof, documentLoader);
            var verified = VerifySignature(verifyData, verificationMethod, proof);
            if (verified)
            {
                return Result.Ok(verificationMethod);
            }
            else
            {
                return Result.Fail("Invalid signature.");
            }
        }

        public override bool MatchProof(Proof proof)
        {
            return proof.Type == Type && proof.CryptoSuiteName == _cryptoSuite.Name;
        }

        public override void EnsureSuiteContext(JObject document, bool addSuiteContext)
        {
            if (IncludesContext(document, ContextUrl) || IncludesContext(document, VC20Context))
            {
                return;
            }
            if (!addSuiteContext)
            {
                throw new ArgumentException($"The document to be signed must contain this suite's @context, {ContextUrl}");
            }
            if (document["@context"] is JArray contextArray)
            {
                contextArray.Add(ContextUrl);
            }
            else if (document["@context"] is JProperty contextString)
            {
                document["@context"] = new JArray(contextString, ContextUrl);
            }
            else
            {
                document["@context"] = ContextUrl;
            }
        }

        private Proof Sign(byte[] verifyData, Proof proof)
        {
            if (_signer is null)
            {
                throw new InvalidOperationException("The signer is not defined.");
            }
            var signatureBytes = _signer.Sign(verifyData);
            Debug.WriteLine($"Signature: {Convert.ToHexString(signatureBytes).ToLower()}");
            proof.ProofValue = $"{MultiBaseBase58BtcHeader}{SimpleBase.Base58.Bitcoin.Encode(signatureBytes)}";
            Debug.WriteLine($"Proof value: {proof.ProofValue}");
            return proof;
        }

        private bool VerifySignature(byte[] verifyData, VerificationMethod verificationMethod, Proof proof)
        {
            Verifier verifier;
            if (_cryptoSuite is ICreateVerifier cv)
            {
                verifier = cv.CreateVerifier(verificationMethod);
            }
            else
            {
                throw new InvalidOperationException($"The cryptosuite {_cryptoSuite.Name} does not support verification.");
            }
            if (_cryptoSuite.RequiredAlgorithm != verifier.Algorithm.ToAlgorithmName())
            {
                throw new ArgumentException($"The verifier's algorithm {verifier.Algorithm.ToAlgorithmName()} does not match the required algorithm for the cryptosuite {_cryptoSuite.RequiredAlgorithm}.");
            }
            var proofValue = proof.ProofValue ?? throw new ArgumentException($"The proof does not include a valid {nameof(proof.ProofValue)} property.");
            var multibaseHeader = proofValue[..1];
            byte[] signature;
            if (multibaseHeader == MultiBaseBase58BtcHeader)
            {
                signature = SimpleBase.Base58.Bitcoin.Decode(proofValue.AsSpan()[1..]);
            }
            else if (multibaseHeader == MultiBaseBase64UrlHeader)
            {
                signature = BaseConvert.FromBase64UrlNoPadding(proofValue[1..]);
            }
            else
            {
                throw new ArgumentException($"Only base58btc or base64url multibase encoding is supported.");
            }
            return verifier.Verify(verifyData, signature);
        }

        private Proof UpdateProof(JObject document, Proof proof, ProofPurpose purpose, IEnumerable<Proof> proofSet, IDocumentLoader documentLoader)
        {
            return proof;
        }

        private byte[] CreateVerifyData(JObject document, Proof proof, IDocumentLoader documentLoader)
        {
            byte[] cachedDocHash;
            if (_hashCache is not null && _hashDocument == document)
            {
                cachedDocHash = _hashCache;
            }
            else
            {
                if (_cryptoSuite is ICanonize cs)
                {
                    _hashDocument = document;
                    Debug.WriteLine($"Document:\n{document}");
                    var canon = cs.Canonize(document, new JsonLdNormalizerOptions { DocumentLoader = documentLoader.LoadDocument });
                    Debug.WriteLine($"Canonized document:\n{canon}");
                    _hashCache = Sha256Digest(canon);
                    cachedDocHash = _hashCache;
                }
                else
                {
                    throw new InvalidOperationException($"The cryptosuite {_cryptoSuite.Name} does not support canonization.");
                }
            }
            var canonizedProof = CanonizeProof(proof, document, documentLoader);
            var proofHash = Sha256Digest(canonizedProof);
            Debug.WriteLine($"Canonized proof:\n{canonizedProof}");
            Debug.WriteLine($"Document hash:\n{Convert.ToHexString(cachedDocHash).ToLower()}");
            Debug.WriteLine($"Proof hash:\n{Convert.ToHexString(proofHash).ToLower()}");
            return [.. proofHash, .. cachedDocHash];
        }

        private static bool IncludesContext(JObject document, string context)
        {
            if (document["@context"] is not JArray contextArray)
            {
                return false;
            }
            return contextArray.Any(c => c.ToString() == context);
        }

        private string CanonizeProof(Proof proof, JObject document, IDocumentLoader documentLoader)
        {
            proof = new Proof(proof)
            {
                Context = document["@context"],
            };
            EnsureSuiteContext(JObject.FromObject(proof), true);
            proof.ProofValue = null;
            if (_cryptoSuite is ICanonize cs)
            {
                var load = JObject.FromObject(proof);
                return cs.Canonize(load, new JsonLdNormalizerOptions { DocumentLoader = documentLoader.LoadDocument });
            }
            else
            {
                throw new InvalidOperationException($"The cryptosuite {_cryptoSuite.Name} does not support canonization.");
            }
        }

        private VerificationMethod GetVerificationMethod(Proof proof, IDocumentLoader documentLoader)
        {
            var verificationMethod = proof.VerificationMethod;
            if (verificationMethod is null)
            {
                throw new ArgumentException($"No {nameof(verificationMethod)} found in proof.");
            }
            try
            {
                var result = documentLoader.LoadDocument(new Uri(verificationMethod), new JsonLdLoaderOptions());
                var doc = result.GetDocument().ToObject<VerificationMethod>();
                if (doc?.Type?.ToLower() == "multikey")
                {
                    doc = result.GetDocument().ToObject<MultikeyVerificationMethod>();
                }
                if (doc is null)
                {
                    throw new ArgumentException($"Could not load verification method {verificationMethod}.");
                }
                return doc;
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Could not load verification method {verificationMethod}.", ex);
            }
        }

        private (Signer? _signer, string? _verificationMethod) ProcessSignatureParams(Signer? signer, string requiredAlgorithm)
        {
            if (signer is null)
            {
                return (null, null);
            }
            if (signer.Algorithm.ToAlgorithmName() != requiredAlgorithm)
            {
                throw new ArgumentException($"The signer's algorithm {signer.Algorithm} does not match the required algorithm for the cryptosuite {requiredAlgorithm}.");
            }
            return (signer, signer.Id);
        }

        private static byte[] Sha256Digest(string message)
        {
            return SHA256.ComputeHash(Encoding.UTF8.GetBytes(message));
        }
    }
}
