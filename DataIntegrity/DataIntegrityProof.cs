using Cryptosuite;
using Cryptosuite.Util;
using ECDsa_2019_Cryptosuite;
using JsonLD.Core;
using JsonLdSignatures;
using JsonLdSignatures.Purposes;
using JsonLdSignatures.Suites;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DataIntegrity
{
    internal class DataIntegrityProof : LinkedDataProof
    {
        private const string ProofType = "DataIntegrityProof";
        private const string DataIntegrityContext = "https://w3id.org/security/data-integrity/v1";
        private const string VC20Context = "https://www.w3.org/ns/credentials/v2";
        private const string MultiBaseBase58BtcHeader = "z";
        private const string MultiBaseBase64UrlHeader = "u";
        private readonly ICryptosuite _cryptoSuite;
        private readonly Signer? _signer;
        private readonly DateTime? _date;
        private readonly string? _verificationMethod;
        private Proof? _proof;
        private byte[]? _hashCache;
        private string? _hashDocument;
        private JsonSerializer _serializer;
        private static readonly SHA256 SHA256 = SHA256.Create();

        public string ContextUrl { get; set; } = DataIntegrityContext;

        public DataIntegrityProof(Signer signer, DateTime date, ICryptosuite cryptoSuite) : base(ProofType)
        {
            _cryptoSuite = cryptoSuite;
            _date = date;
            _signer = signer;
            _verificationMethod = signer?.VerificationMethod;
            if (_signer?.Algorithm != _cryptoSuite.RequiredAlgorithm)
            {
                throw new ArgumentException($"The signer's algorithm {_signer?.Algorithm} does not match the required algorithm for the cryptosuite {_cryptoSuite.RequiredAlgorithm}.");
            }
            _serializer = new JsonSerializer();
            _serializer.Converters.Add(new SingleArrayConverter<string>());
        }

        public override object CreateProof(string document, ProofPurpose purpose, ProofSet proofSet, DocumentLoader documentLoader)
        {
            var proof = _proof is null ? new Proof() : new Proof(_proof);
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
            if (_cryptoSuite is ICreateProofValue csp)
            {
                proof.ProofValue = csp.CreateProofValue(verifyData, document, proof, proofSet, documentLoader);
            }
            else
            {
                proof = Sign(verifyData, proof);
            }
            return proof;
        }

        public override object Derive(string document, ProofPurpose purpose, ProofSet proofSet, DocumentLoader documentLoader)
        {
            throw new NotImplementedException();
        }

        public override bool VerifyProof(Proof proof, string document, ProofPurpose purpose, ProofSet proofSet, DocumentLoader documentLoader)
        {
            throw new NotImplementedException();
        }

        public Proof Sign(object verifyData, Proof proof)
        {
            if (_signer is null)
            {
                throw new InvalidOperationException("The signer is not defined.");
            }
            var signatureBytes = _signer.Sign(verifyData);
            proof.ProofValue = MultiBaseBase58BtcHeader + SimpleBase.Base58.Bitcoin.Encode(signatureBytes);
            return proof;
        }

        public bool VerifySignature(object verifyData, string verificationMethod, Proof proof)
        {
            var verifier = _cryptoSuite.CreateVerifier(verificationMethod);
            if (_cryptoSuite.RequiredAlgorithm != verifier.Algorithm)
            {
                throw new ArgumentException($"The verifier's algorithm {verifier.Algorithm} does not match the required algorithm for the cryptosuite {_cryptoSuite.RequiredAlgorithm}.");
            }
            var proofValue = proof.ProofValue ?? throw new ArgumentException($"The proof does not include a valid {nameof(proof.ProofValue)} property.");
            var multibaseHeader = proofValue.Substring(0, 1);
            byte[] signature;
            if (multibaseHeader == MultiBaseBase58BtcHeader)
            {
                signature = SimpleBase.Base58.Bitcoin.Decode(proofValue.AsSpan()[1..]);
            }
            else if (multibaseHeader == MultiBaseBase64UrlHeader)
            {
                signature = Convert.FromBase64String(proofValue[1..]);
            }
            else
            {
                throw new ArgumentException($"Only base58btc or base64url multibase encoding is supported.");
            }
            return verifier.Verify(verifyData, signature);
        }

        public Proof UpdateProof(string document, Proof proof, ProofPurpose purpose, ProofSet proofSet, DocumentLoader documentLoader)
        {
            return proof;
        }

        public byte[] CreateVerifyData(string document, Proof proof, DocumentLoader documentLoader)
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
                    var canon = cs.Canonize(document, new JsonLdOptions { documentLoader = documentLoader });
                    _hashCache = Sha256Digest(canon);
                    cachedDocHash = _hashCache;
                }
                else
                {
                    throw new InvalidOperationException($"The cryptosuite {_cryptoSuite.Name} does not support canonization.");
                }
            }
            var proofHash = Sha256Digest(CanonizeProof(proof, document, documentLoader));
            return cachedDocHash.Concat(proofHash).ToArray();
        }

        private string CanonizeProof(Proof proof, string document, DocumentLoader documentLoader)
        {
            var jDoc = JObject.Parse(document);
            proof.Context = jDoc["@context"]?.ToObject<IEnumerable<string>>(_serializer);
            EnsureSuiteContext(proof, true);
            proof.ProofValue = null;
            if (_cryptoSuite is ICanonize cs)
            {
                var load = JsonConvert.SerializeObject(proof);
                return cs.Canonize(load, new JsonLdOptions { documentLoader = documentLoader });
            }
            else
            {
                throw new InvalidOperationException($"The cryptosuite {_cryptoSuite.Name} does not support canonization.");
            }
        }

        private void EnsureSuiteContext(Proof proof, bool addSuiteContext)
        {
            if ((proof.Context?.Contains(ContextUrl) ?? false) || (proof.Context?.Contains(VC20Context) ?? false))
            {
                return;
            }
            if (addSuiteContext)
            {
                proof.Context = proof.Context?.Append(ContextUrl);
            }
            else
            {
                throw new ArgumentException($"The document to be signed must contain this suite's @context: {ContextUrl}.");
            }
        }

        private static byte[] Sha256Digest(string message)
        {
            return SHA256.ComputeHash(Encoding.UTF8.GetBytes(message));
        }
    }
}
