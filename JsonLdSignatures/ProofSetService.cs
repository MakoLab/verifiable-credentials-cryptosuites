using Cryptosuite.Core;
using FluentResults;
using JsonLdExtensions;
using JsonLdSignatures.Purposes;
using JsonLdSignatures.Suites;
using Newtonsoft.Json.Linq;

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

        public IEnumerable<Result<VerificationResult>> Verify(JObject document, LinkedDataSignature[] suites, IList<ProofPurpose> purposes, IDocumentLoader documentLoader)
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
                return new List<Result<VerificationResult>>() { Result.Fail("No matching proofs found in the given document.") };
            }
            doc.Remove("proof");
            var results = Verify(document, suites, proofSet, purposes, loader);
            if (results.Count == 0)
            {
                return new List<Result<VerificationResult>>() { Result.Fail("Did not verify any proofs; insufficient proofs matched the acceptable suite(s) and required purpose(s).") };
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
            var context = document["@context"] ?? new JValue(Contexts.SecurityContextUrl);
            foreach (var proof in proofSet)
            {
                proof.Context = context;
            }
            return proofSet;
        }

        private List<Result<VerificationResult>> Verify(JObject document, LinkedDataSignature[] suites, IEnumerable<Proof> proofSet, IList<ProofPurpose> purposes, IDocumentLoader documentLoader)
        {
            var purposeToProofs = new Dictionary<ProofPurpose, List<Proof>>();
            var proofToSuite = new Dictionary<Proof, LinkedDataSignature>();
            foreach (var purpose in purposes)
            {
                MatchProofSet(proofSet, suites, purpose, purposeToProofs, proofToSuite);
            }
            if (purposeToProofs.Count < purposes.Count)
            {
                return [Result.Fail("Insufficient proofs matched the acceptable suite(s) and required purpose(s).")];
            }
            var results = new List<Result<VerificationResult>>();
            foreach (var (proof, suite) in proofToSuite)
            {
                var purpose = new ProofPurpose(proof.Type, DateTime.UtcNow);
                var result = suite.VerifyProof(proof, document, purpose, proofSet, documentLoader);
                if (result.IsSuccess)
                {
                    results.Add(Result.Ok(new VerificationResult() { Proof = proof, VerificationMethod = result.Value }));
                }
                else
                {
                    var r = Result.Fail<VerificationResult>(result.Errors.First());
                    //r.Value.PurposeValidation = new List<Result<ValidationResult>>();
                    results.Add(r);
                }
            }
            foreach (var (purpose, proofs) in purposeToProofs)
            {
                foreach (var proof in proofs)
                {
                    var result = results.FirstOrDefault(r => r.IsSuccess && r.Value.Proof == proof);
                    if (result is null || !result.IsSuccess)
                    {
                        continue;
                    }
                    var vm = result.Value.VerificationMethod;
                    if (vm is null)
                    {
                        throw new Exception("Verification method is required.");
                    }
                    var suite = proofToSuite[proof];
                    var purposeResult = ((ControllerProofPurpose)purpose).Validate(proof, vm, documentLoader);
                    result.Value.PurposeValidation.Add(purposeResult);
                }
            }
            return results;
        }

        private void MatchProofSet(
            IEnumerable<Proof> proofSet,
            LinkedDataSignature[] suites,
            ProofPurpose purpose,
            Dictionary<ProofPurpose, List<Proof>> purposeToProofs,
            Dictionary<Proof, LinkedDataSignature> proofToSuite)
        {
            foreach (var proof in proofSet)
            {
                if (!purpose.Match(proof))
                {
                    continue;
                }
                var matched = false;
                foreach (var suite in suites)
                {
                    if (suite.MatchProof(proof))
                    {
                        matched = true;
                        proofToSuite.Add(proof, suite);
                        break;
                    }
                }
                if (matched)
                {
                    var matches = purposeToProofs.ContainsKey(purpose) ? purposeToProofs[purpose] : new List<Proof>();
                    matches.Add(proof);
                    purposeToProofs[purpose] = matches;
                }
            }
        }
    }
}
