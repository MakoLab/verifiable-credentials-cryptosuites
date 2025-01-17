using Cryptosuite.Core;
using Cryptosuite.Core.Interfaces;
using DI_Sd_Primitives;
using ECDsa_Multikey;
using ECDsa_sd_2023_Functions;
using JsonLdExtensions;
using JsonLdExtensions.Canonicalization;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Text;

namespace ECDsa_sd_2023_Cryptosuite
{
    public class ECDsaSd2023CreateProofCryptosuite : ICryptosuite, ICreateProofValue, ICanonize, ICreateVerifyData
    {
        public string RequiredAlgorithm { get => "P-256"; }
        public string Name { get => "ecdsa-sd-2023"; }
        public static string TypeName { get => "ecdsa-sd-2023-create"; }
        public IList<string> MandatoryPointers { get; set; }

        public ECDsaSd2023CreateProofCryptosuite()
        {
            MandatoryPointers = [];
        }

        public ECDsaSd2023CreateProofCryptosuite(IList<string> mandatoryPointers)
        {
            MandatoryPointers = mandatoryPointers;
        }

        public string Canonize(JToken input, JsonLdNormalizerOptions options)
        {
            return JsonLdNormalizer.Normalize(input, options).SerializedNQuads.Replace("\r", String.Empty);
        }

        public string CreateProofValue(byte[] verifyData, JObject document, Proof proof, IEnumerable<Proof> proofSet, IDocumentLoader documentLoader, Signer signer)
        {
            var hs = new HashingService();
            var (hMac, hmacKey) = hs.CreateHMAC();
            var hlmff = new HMACLabelMapFactoryFunction(hMac);
            var groupDefinitions = new Dictionary<string, IList<string>>
            {
                { "mandatory", MandatoryPointers }
            };
            var cResult = document.CanonicalizeAndGroup(hlmff, groupDefinitions);
            var mandatory = new List<string>();
            var nonMandatory = new List<string>();
            if (cResult.Groups.TryGetValue("mandatory", out var value) && value.Matching.Count > 0)
            {
                mandatory = [.. value.Matching.Values];
                nonMandatory = [.. value.NonMatching.Values];
            }
            proof.Context = document["@context"];
            var canon = Canonize(JObject.FromObject(proof), new JsonLdNormalizerOptions() { DocumentLoader = documentLoader.LoadDocument });
            var proofHash = hs.HashString(canon);
            var mandatoryHash = hs.HashMandatoryNQuads(mandatory);

            var proofScopedKeyPair = MultikeyService.Generate(null, null, ECDsaCurveType.P256);
            if (proofScopedKeyPair.Signer is null)
            {
                throw new Exception("Signer is null");
            }
            var signatures = new List<byte[]>();
            foreach (var item in nonMandatory)
            {
                signatures.Add(proofScopedKeyPair.Signer.Sign(Encoding.UTF8.GetBytes(item)));
            }
            var publicKey = MultikeyService.ToByteArray(proofScopedKeyPair.KeyPair.GetPublicKeyMultibase() ?? throw new Exception("No public key in Verification Method"));
            var toSign = BaseProof.SerializeSignData(proofHash, publicKey, mandatoryHash);
            var baseSignature = signer.Sign(toSign);
            var baseProof = new BaseProof()
            {
                PublicKey = publicKey,
                HMACKey = hmacKey,
                MandatoryPointers = MandatoryPointers,
                Signatures = signatures,
                BaseSignature = baseSignature
            };
            #region Debug
            Debug.WriteLine($"Public key:\n{Convert.ToHexString(publicKey)}");
            Debug.WriteLine($"Base signature:\n{Convert.ToHexString(baseSignature)}");
            Debug.WriteLine($"Signatures:\n{string.Join("\n", signatures.Select(s => Convert.ToHexString(s)))}");
            Debug.WriteLine($"Mandatory:\n{string.Join("", mandatory)}");
            Debug.WriteLine($"Non-mandatory:\n{string.Join("", nonMandatory)}");
            Debug.WriteLine($"Canonicalized:\n{canon}");
            Debug.WriteLine($"Proof hash:\n{Convert.ToHexString(proofHash)}");
            Debug.WriteLine($"Mandatory hash:\n{Convert.ToHexString(mandatoryHash)}");
            Debug.WriteLine($"To sign:\n{Convert.ToHexString(toSign)}");
            Debug.WriteLine($"HMAC key:\n{Convert.ToHexString(hmacKey)}");
            #endregion
            return baseProof.Serialize();
        }

        public byte[] CreateVerifyData(JObject document, Proof proof, IEnumerable<Proof> proofSet, IDocumentLoader documentLoader)
        { 
            return [];
        }
    }
}
