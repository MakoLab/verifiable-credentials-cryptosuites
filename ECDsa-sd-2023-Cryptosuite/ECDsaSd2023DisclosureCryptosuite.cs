using Cryptosuite.Core;
using Cryptosuite.Core.Interfaces;
using ECDsa_sd_2023_Functions;
using JsonLdExtensions;
using Newtonsoft.Json.Linq;

namespace ECDsa_sd_2023_Cryptosuite
{
    public class ECDsaSd2023DisclosureCryptosuite : IDerive
    {
        public string RequiredAlgorithm { get => "P-256"; }
        public string Name { get => "ecdsa-sd-2023"; }
        private IList<string> SelectivePointers { get; set; }

        public ECDsaSd2023DisclosureCryptosuite()
        {
            SelectivePointers = [];
        }

        public ECDsaSd2023DisclosureCryptosuite(IList<string> selectivePointers)
        {
            SelectivePointers = selectivePointers;
        }

        public JObject Derive(JObject document, IProofPurpose proofPurpose, IEnumerable<Proof> proofSet, IDocumentLoader documentLoader)
        {
            var proof = FindProof(proofSet);
            var dd = DerivedProofService.CreateDisclosureData(document, proof, SelectivePointers);
            if (dd.RevealDocument is null)
            {
                throw new Exception("No selective disclosure data found.");
            }
            var newProof = new Proof(proof);
            var pv = dd.SerializeDerivedProofValue();
            newProof.ProofValue = pv;
            dd.RevealDocument.Add("proof", JObject.FromObject(newProof));
            return dd.RevealDocument;
        }

        private Proof FindProof(IEnumerable<Proof> proofSet)
        {
            var proofs = proofSet.Where(p => p.Type == Name).ToList();
            if (proofs.Count == 0)
            {
                throw new Exception($"No matching base proof found from which to derive a disclosure proof.");
            }
            if (proofs.Count > 1)
            {
                throw new Exception("Multiple matching proofs; a 'proofId' must be specified.");
            }
            return proofs[0];
        }
    }
}
