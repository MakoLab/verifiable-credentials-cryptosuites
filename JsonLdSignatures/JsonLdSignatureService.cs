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
    public class JsonLdSignatureService
    {
        private readonly ProofSetService _proofSetService;
        public JsonLdSignatureService(ProofSetService? proofSetService = null)
        {
            _proofSetService = proofSetService ?? new ProofSetService();
        }

        public JObject Derive(JObject document, LinkedDataSignature suite, ProofPurpose purpose, IDocumentLoader documentLoader, bool addSuiteContext = true)
        {
            suite.EnsureSuiteContext(document, addSuiteContext);
            return _proofSetService.Derive(document, suite, purpose, documentLoader);
        }

        public JObject Sign(JObject document, LinkedDataSignature suite, ProofPurpose purpose, IDocumentLoader documentLoader, bool addSuiteContext = true)
        {
            suite.EnsureSuiteContext(document, addSuiteContext);
            return _proofSetService.Add(document, suite, purpose, documentLoader);
        }

        public IEnumerable<Result<VerificationResult>> Verify(JObject document, LinkedDataSignature[] suites, IList<ProofPurpose> purposes, IDocumentLoader documentLoader)
        {
            return _proofSetService.Verify(document, suites, purposes, documentLoader);
        }

        public IEnumerable<Result<VerificationResult>> Verify(JObject document, LinkedDataSignature suite, ProofPurpose purpose, IDocumentLoader documentLoader)
        {
            return _proofSetService.Verify(document, new[] { suite }, new List<ProofPurpose>() { purpose }, documentLoader);
        }

        public JObject ToJsonResult(IEnumerable<Result<VerificationResult>> results)
        {
            var json = new JObject();
            var array = new JArray();
            var errors = new JArray();
            foreach (var result in results)
            {
                var verificationResult = result.ValueOrDefault;
                if (verificationResult != null)
                {
                    var verificationResultJson = JObject.FromObject(verificationResult);
                    array.Add(verificationResultJson);
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        errors.Add(JValue.CreateString(error.Message));
                    }
                }
            }
            json.Add("results", array);
            json.Add("errors", errors);
            if (array.Count > 0)
            {
                json.Add("verified", true);
            }
            else
            {
                json.Add("verified", false);
            }
            return json;
        }
    }
}
