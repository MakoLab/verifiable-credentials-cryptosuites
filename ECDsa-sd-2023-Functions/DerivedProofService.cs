using Cryptosuite.Core;
using DI_Sd_Primitives;
using JsonLdExtensions.Canonicalization;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Parameters;
using VDS.RDF;

namespace ECDsa_sd_2023_Functions
{
    public class DerivedProofService
    {
        /// <summary>
        /// Creates data to be used to generate a derived proof.
        /// </summary>
        /// <param name="document">JSON-LD document.</param>
        /// <param name="proof">ECDSA-SD base proof.</param>
        /// <param name="selectivePointers">An array of JSON pointers to use to selectively disclose statements.</param>
        /// <returns>Disclosure data, which contains the "baseSignature", "publicKey", "signatures", "labelMap",
        /// "mandatoryIndexes", and "revealDocument" fields.</returns>
        public static DisclosureData CreateDisclosureData(JObject document, Proof proof, IList<string> selectivePointers)
        {
            if (proof.ProofValue == null)
            {
                throw new ArgumentException("Proof value is required to create disclosure data.");
            }
            var baseProof = BaseProof.Deserialize(proof.ProofValue);
            var hmac = new HMac(new Sha256Digest());
            hmac.Init(new KeyParameter(baseProof.HMACKey));
            var labelMapFactory = new HMACLabelMapFactoryFunction(hmac);
            var combinedPointers = baseProof.MandatoryPointers.Concat(selectivePointers).ToList();
            var groupDefinitions = new Dictionary<string, IList<string>>
            {
                { "mandatory", baseProof.MandatoryPointers },
                { "selective", selectivePointers },
                { "combined", combinedPointers }
            };
            var canonicalizedGroups = JsonSelectionExtension.CanonicalizeAndGroup(document, labelMapFactory, groupDefinitions);
            var relativeIndex = 0;
            var mandatoryIndexes = new List<int>();
            foreach (var absoluteIndex in canonicalizedGroups.Groups["combined"].Matching.Keys)
            {
                if (canonicalizedGroups.Groups["mandatory"].Matching.ContainsKey(absoluteIndex))
                {
                    mandatoryIndexes.Add(relativeIndex);
                    relativeIndex++;
                }
            }
            var index = 0;
            var filteredSignatures = new List<byte[]>();
            foreach (var signature in baseProof.Signatures)
            {
                while (canonicalizedGroups.Groups["mandatory"].Matching.ContainsKey(index))
                {
                    index++;
                }
                if (canonicalizedGroups.Groups["selective"].Matching.ContainsKey(index))
                {
                    filteredSignatures.Add(signature);
                }
                index++;
            }
            var revealDocument = document.SelectJsonLd(combinedPointers);
            var ts = new TripleStore();
            ts.LoadFromString(String.Join('\n', canonicalizedGroups.Groups["combined"].DeskolemizedNQuads));
            var canonicalizer = new RdfCanonicalizer();
            var nts = canonicalizer.Canonicalize(ts);
            var verifierLabelMap = new Dictionary<string, string>();
            foreach (var key in nts.IssuedIdentifiersMap.Keys)
            {
                verifierLabelMap.Add(nts.IssuedIdentifiersMap[key]!.ToString()!, canonicalizedGroups.LabelMap[key.ToString()!]);
            }
            return new DisclosureData
            {
                BaseSignature = baseProof.BaseSignature,
                PublicKey = baseProof.PublicKey,
                Signatures = filteredSignatures,
                LabelMap = verifierLabelMap,
                MandatoryIndexes = mandatoryIndexes,
                RevealDocument = revealDocument
            };
        }

        /// <summary>
        /// Creates the data needed to perform verification of an ECDSA-SD-protected verifiable credential.
        /// </summary>
        /// <param name="document">A JSON-LD document.</param>
        /// <param name="proof">An ECDSA-SD disclosure proof.</param>
        /// <returns>A single verify data object value.</returns>
        public static VerifyData CreateVerifyData(JObject document, Proof proof)
        {
            if (proof.ProofValue == null)
            {
                throw new ArgumentException("Proof value is required to create verify data.");
            }
            var proofCopy = new Proof(proof)
            {
                Type = "DataIntegrityProof",
                ProofValue = null
            };
            var proofDocument = JObject.FromObject(proofCopy);
            var canonicalized = JsonLdNormalizer.Normalize(proofDocument, new JsonLdNormalizerOptions()).SerializedNQuads.Replace("\r", String.Empty);
            var proofHash = new HashingService().HashString(canonicalized);
            var derivedProof = DisclosureData.ParseDerivedProofValue(proof.ProofValue);
            var labelMapFunction = new CustomLabelMapFactoryFunction(derivedProof.LabelMap);
            var cs = new CanonicalizationService();
            var nquads = cs.LabelReplacementCanonicalize(document, labelMapFunction).canonicalNQuads;
            var mandatory = new List<string>();
            var nonMandatory = new List<string>();
            for (var i = 0; i < nquads.Count; i++)
            {
                if (derivedProof.MandatoryIndexes.Contains(i))
                {
                    mandatory.Add(nquads[i]);
                }
                else
                {
                    nonMandatory.Add(nquads[i]);
                }
            }
            var hs = new HashingService();
            var mandatoryHash = hs.HashMandatoryNQuads(mandatory);
            return new VerifyData
            {
                BaseSignature = derivedProof.BaseSignature,
                ProofHash = proofHash,
                PublicKey = derivedProof.PublicKey,
                Signatures = derivedProof.Signatures,
                NonMandatory = nonMandatory,
                MandatoryHash = mandatoryHash
            };
        }
    }
}
