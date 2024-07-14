using Cryptosuite.Core;
using Cryptosuite.Core.Interfaces;
using ECDsa_Multikey;
using ECDsa_sd_2023_Functions;
using JsonLdExtensions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDsa_sd_2023_Cryptosuite
{
    public class ECDsaSd2023VerifyCryptosuite : ICryptosuite, ICreateVerifier, ICreateVerifyData
    {
        public string RequiredAlgorithm { get => "P-256"; }
        public string Name { get => "ecdsa-sd-2023"; }

        public Verifier CreateVerifier(VerificationMethod verificationMethod)
        {
            if (verificationMethod.Type?.ToLower() != "multikey")
            {
                throw new Exception("VerificationMethod must be a MultikeyModel");
            }
            var key = Multikey.From((MultikeyModel)verificationMethod);
            return key.Verifier;
        }

        public byte[] CreateVerifyData(JObject document, Proof proof, IEnumerable<Proof> proofSet, IDocumentLoader documentLoader)
        {
            var vd = DerivedProofService.CreateVerifyData(document, proof);
            if (vd.Signatures.Count != vd.NonMandatory.Count)
            {
                throw new Exception("Number of signatures does not match number of non-mandatory statements.");
            }
            var publicKeyBytes = vd.PublicKey;
            var toVerify = BaseProof.SerializeSignData(vd.ProofHash, vd.PublicKey, vd.MandatoryHash);
            var verified = true;
            var vm = new VerificationMethod()
            {
                Id = proof.VerificationMethod,
                Type = "Multikey",
            };
            var verifier = CreateVerifier(vm);
            for (int i = 0; i < vd.Signatures.Count; i++)
            {
                var signature = vd.Signatures[i];
                byte[] verifyData = Encoding.UTF8.GetBytes(vd.NonMandatory[i]);
                var verificationCheck = verifier.Verify(verifyData, signature);
                if (!verificationCheck)
                {
                    verified = false;
                    break;
                }
            }
            if (!verified)
            {
                throw new Exception("Signature verification failed.");
            }
            return toVerify;
        }
    }
}
