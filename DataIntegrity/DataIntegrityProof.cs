using Cryptosuite;
using ECDsa_2019_Cryptosuite;
using JsonLdSignatures;
using JsonLdSignatures.Purposes;
using JsonLdSignatures.Suites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataIntegrity
{
    internal class DataIntegrityProof : LinkedDataProof
    {
        private const string ProofType = "DataIntegrityProof";
        private const string DataIntegrityContext = "https://w3id.org/security/data-integrity/v1";
        private const string MultiBaseBase58BtcHeader = "z";
        private const string MultiBaseBase64UrlHeader = "u";
        private readonly ICryptosuite _cryptoSuite;
        private readonly Signer? _signer;
        private readonly DateTime? _date;
        private readonly string? _verificationMethod;
        private Proof? _proof;

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
        }

        public override object CreateProof(Document document, ProofPurpose purpose, ProofSet proofSet, DocumentLoader documentLoader)
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

            throw new NotImplementedException();
        }

        public override object Derive(Document document, ProofPurpose purpose, ProofSet proofSet, DocumentLoader documentLoader)
        {
            throw new NotImplementedException();
        }

        public override bool VerifyProof(Proof proof, Document document, ProofPurpose purpose, ProofSet proofSet, DocumentLoader documentLoader)
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

        public Proof UpdateProof(Document document, Proof proof, ProofPurpose purpose, ProofSet proofSet, DocumentLoader documentLoader)
        {
            return proof;
        }
    }
}
