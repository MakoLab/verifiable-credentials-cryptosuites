using Cryptosuite.Core;
using Cryptosuite.Core.ControllerDocuments;
using Cryptosuite.Core.Interfaces;
using ECDsa_Multikey;
using ECDsa_sd_2023_Functions;
using JsonLdExtensions;
using Newtonsoft.Json.Linq;
using System.Text;

namespace ECDsa_sd_2023_Cryptosuite
{
    public class ECDsaSd2023VerifyCryptosuite : ICryptosuite, ICreateVerifier, ICreateVerifyData
    {
        public string RequiredAlgorithm { get => "P-256"; }
        public string Name { get => "ecdsa-sd-2023"; }
        public static string TypeName { get => "ecdsa-sd-2023-verify"; }

        public Verifier CreateVerifier(VerificationMethod verificationMethod)
        {
            if (verificationMethod.Type?.ToLower() != "multikey")
            {
                throw new Exception("VerificationMethod must be a MultikeyModel");
            }
            var key = MultikeyService.From((MultikeyVerificationMethod)verificationMethod);
            return key.Verifier;
        }

        public byte[] CreateVerifyData(JObject document, Proof proof, IEnumerable<Proof> proofSet, IDocumentLoader documentLoader)
        {
            if (proof.VerificationMethod is null)
            {
                throw new Exception("VerificationMethod is required.");
            }
            var vd = DerivedProofService.CreateVerifyData(document, proof);
            if (vd.Signatures.Count != vd.NonMandatory.Count)
            {
                throw new Exception("Number of signatures does not match number of non-mandatory statements.");
            }
            var controllerDocument = GetControllerDocument(proof.VerificationMethod, documentLoader);
            var vm = GetVerificationMethod(proof.VerificationMethod, controllerDocument);
            var publicKeyBytes = vd.PublicKey;
            var toVerify = BaseProof.SerializeSignData(vd.ProofHash, vd.PublicKey, vd.MandatoryHash);
            var verified = true;
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

        private ControllerDocument GetControllerDocument(string uri, IDocumentLoader documentLoader)
        {
            var remoteDocument = documentLoader.LoadDocument(new Uri(uri));
            var controllerDocument = remoteDocument.Document switch
            {
                JToken jToken => jToken.ToObject<ControllerDocument>(),
                string str => JObject.Parse(str).ToObject<ControllerDocument>(),
                _ => throw new Exception("Invalid remote document type.")
            };
            return controllerDocument is not null ? controllerDocument : throw new Exception("Could not retrieve a controller document from url.");
        }

        private VerificationMethod GetVerificationMethod(string uri, ControllerDocument cd)
        {
            var vm = cd.VerificationMethod?.FirstOrDefault(vm => vm.Id == uri);
            return vm is not null ? vm : throw new Exception("Could not retrieve a verification method from controller document.");
        }
    }
}
