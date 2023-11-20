using Cryptosuite.Core;
using FluentResults;
using JsonLdExtensions;
using JsonLdSignatures.Purposes;
using JsonLdSignatures.Suites;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonLdSignatures
{
    public class ProofSetService
    {
        public JObject Add(JObject document, LinkedDataSignature suite, ProofPurpose purpose, IDocumentLoader documentLoader)
        {
            IDocumentLoader loader;
            if (documentLoader is null)
            {
                loader = new ExtendedDocumentLoader(new StrictDocumentLoader());
            }
            else
            {
                loader = new ExtendedDocumentLoader(documentLoader);
            }
            var doc = (JObject)document.DeepClone();
            doc.Remove("proof");
            var proofSet = GetProofs(doc);
            var proof = suite.CreateProof(doc, purpose, proofSet, loader);
            doc.Add("proof", JToken.FromObject(proof));
            return doc;
        }

        public JObject Derive(JObject document, LinkedDataSignature suite, ProofPurpose purpose, IDocumentLoader documentLoader)
        {
            IDocumentLoader loader;
            if (documentLoader is null)
            {
                loader = new ExtendedDocumentLoader(new StrictDocumentLoader());
            }
            else
            {
                loader = new ExtendedDocumentLoader(documentLoader);
            }
            var doc = (JObject)document.DeepClone();
            doc.Remove("proof");
            var proofSet = GetProofs(doc);
            var newDocument = suite.Derive(doc, purpose, proofSet, loader);
            return newDocument;
        }

        public IEnumerable<Result> Verify(JObject document, LinkedDataSignature[] suites, ProofPurpose purpose, IDocumentLoader documentLoader)
        {
            IDocumentLoader loader;
            if (documentLoader is null)
            {
                loader = new ExtendedDocumentLoader(new StrictDocumentLoader());
            }
            else
            {
                loader = new ExtendedDocumentLoader(documentLoader);
            }
            var doc = (JObject)document.DeepClone();
            var proofSet = GetProofs(doc);
            if (!proofSet.Any())
            {
                return new List<Result>() { Result.Fail("No matching proofs found in the given document.") };
            }
            doc.Remove("proof");
            var results = Verify(document, suites, proofSet, purpose, loader);
            if (!results.Any())
            {
                return new List<Result>() { Result.Fail("Did not verify any proofs; insufficient proofs matched the acceptable suite(s) and required purpose(s).") };
            }
            return results;
        }

        private IEnumerable<Proof> GetProofs(JObject document)
        {
            var proofSet = new List<Proof>();
            var proofs = document["proof"];
            if (proofs is null)
            {
                return proofSet;
            }
            if (proofs is JArray)
            {
                proofSet.AddRange(proofs.Select(p => p.ToObject<Proof>()).Where(p => p != null)!);
            }
            else
            {
                var proof = proofs.ToObject<Proof>();
                if (proof != null)
                {
                    proofSet.Add(proof);
                }
            }
            var context = document["@context"] ?? new JValue(SecurityConstants.SecurityContextUrl);
            foreach (var proof in proofSet)
            {
                proof.Context = context;
            }
            return proofSet;
        }

        private IEnumerable<Result> Verify(JObject document, LinkedDataSignature[] suites, IEnumerable<Proof> proofSet, ProofPurpose purpose, IDocumentLoader documentLoader)
        {
            throw new NotImplementedException();
        }
    }
}
