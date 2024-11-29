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
            var scopedPublickey = MultikeyService.ToMultibaseString(vd.PublicKey);
            var baseSignature = MultikeyService.ToMultibaseString(vd.BaseSignature);
            var scopedVm = GetVerificationMethodDocument($"did:key:{scopedPublickey}#{scopedPublickey}", documentLoader);
            var toVerify = BaseProof.SerializeSignData(vd.ProofHash, vd.PublicKey, vd.MandatoryHash);
            var verified = true;
            var verifier = CreateVerifier(scopedVm);
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
                throw new Exception("Non-mandatory properties verification failed.");
            }
            proof.ProofValue = baseSignature;
            return toVerify;
        }

        private VerificationMethod GetVerificationMethodDocument(string uri, IDocumentLoader documentLoader)
        {
            var url = new Uri(uri);
            var remoteDocument = documentLoader.LoadDocument(url);
            var document = remoteDocument.Document switch
            {
                JObject jObject => jObject,
                string str => JObject.Parse(str),
                _ => throw new Exception("Invalid remote document type.")
            };
            var documentType = GetDocumentType(document) ?? throw new Exception("Invalid document type.");
            if (documentType == typeof(MultikeyVerificationMethod))
            {
                return document.ToObject<MultikeyVerificationMethod>() ?? throw new Exception("Invalid MultikeyVerificationMethod document.");
            }
            if (documentType == typeof(ControllerDocument))
            {
                var cd = document.ToObject<ControllerDocument>() ?? throw new Exception("Invalid ControllerDocument document.");
                var keyId = url.Fragment.Length > 0 ? url.Fragment[1..] : String.Empty;
                if (keyId.Length > 0)
                {
                    return GetVerificationMethod(uri, cd);
                }
                else
                {
                    return cd.VerificationMethod?.FirstOrDefault() ?? throw new Exception("Verification method not found in controller document.");
                }
            }
            throw new Exception("Invalid document type.");
        }

        private static Type? GetDocumentType(JObject document)
        {
            if (document["type"]?.ToString().ToLower() == "multikey")
            {
                return typeof(MultikeyVerificationMethod);
            }
            if (document["verificationMethod"] is not null)
            {
                return typeof(ControllerDocument);
            }
            return null;
        }

        private VerificationMethod GetVerificationMethod(string uri, ControllerDocument cd)
        {
            var vm = cd.VerificationMethod?.FirstOrDefault(vm => vm.Id == uri);
            return vm is not null ? vm : throw new Exception("Verification method not found in controller document.");
        }
    }
}
