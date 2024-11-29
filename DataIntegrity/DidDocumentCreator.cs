using Cryptosuite.Core;
using Cryptosuite.Core.ControllerDocuments;
using ECDsa_Multikey;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataIntegrity
{
    public class DidDocumentCreator : IDidDocumentCreator
    {
        /// <summary>
        /// Create a DID Document from a DID URI.
        /// </summary>
        /// <param name="uri">Document Uri</param>
        /// <returns>Controller document or Verification method document.</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <remarks>Supports "key" method only.</remarks>
        public BaseDocument CreateControllerDocument(Uri uri)
        {
            if (uri.Scheme != "did")
                throw new ArgumentException("URI must be a DID");
            var method = uri.LocalPath.Split(":").First();
            if (method != "key")
                throw new ArgumentException("URI must be a key method DID");
            var keyId = uri.Fragment.Length > 0 ? uri.Fragment[1..] : String.Empty;
            var publicKeyMultibase = uri.LocalPath.Split(":").Last();
            var keypair = MultikeyService.From(new MultikeyVerificationMethod
            {
                PublicKeyMultibase = publicKeyMultibase,
            });
            publicKeyMultibase = keypair.KeyPair.GetPublicKeyMultibase();
            var did = $"did:key:{publicKeyMultibase}";
            keypair.KeyPair.Controller = did;
            keypair.KeyPair.Id = $"{did}#{publicKeyMultibase}";
            var verificationPublicKey = keypair.KeyPair.Export(ExportKeyPairOptions.IncludePublicKey | ExportKeyPairOptions.IncludeContext);
            var didDocument = new ControllerDocument(did)
            {
                Context = AddContexts(Contexts.DidContextV1Url, verificationPublicKey.Context),
                VerificationMethod = [verificationPublicKey],
                Authentication = [verificationPublicKey.Id!],
                AssertionMethod = [verificationPublicKey.Id!],
            };
            if (keyId.Length > 0)
            {
                verificationPublicKey.Context = Constants.MultikeyContextV1Url;
                return verificationPublicKey;
            }
            return didDocument;
        }

        private static JToken? AddContexts(JToken? context, JToken? added)
        {
            if (added is null)
                return context;
            if (context is null)
                return added;
            if (context is JArray array)
            {
                array.Add(added);
                return array;
            }
            return new JArray(context, added);
        }
    }
}
