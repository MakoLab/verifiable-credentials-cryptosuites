using Cryptosuite.Core;
using FluentResults;
using JsonLdExtensions;
using JsonLdSignatures.Purposes;
using JsonLdSignatures.Suites;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
            var goods = new JArray();
            var errors = new JArray();
            foreach (var result in results)
            {
                var verificationResult = result.ValueOrDefault;
                if (verificationResult != null)
                {
                    var verificationResultJson = JObject.FromObject(verificationResult);
                    goods.Add(verificationResultJson);
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        errors.Add(JValue.CreateString(error.Message));
                    }
                }
            }
            if (goods.Count > 0)
            {
                json.Add("verified", true);
                var res = new JObject
                {
                    { "results", goods },
                    { "status", 200 }
                };
                json.Add("result", res);
            }
            else
            {
                json.Add("verified", false);
                var res = new JObject
                {
                    { "errors", errors },
                    { "status", 400 }
                };
                json.Add("error", res);
            }
            return json;
        }

        public JObject ToJsonResult(string message, HttpStatusCode status)
        {
            var json = new JObject();
            if (status == HttpStatusCode.OK)
            {
                var res = new JObject()
                {
                    { "status", (int)status },
                    { "results", new JArray() { message } }
                };
                json.Add("result", res);
            }
            else
            {
                var res = new JObject
                {
                    { "status", (int)status },
                    { "errors", new JArray() { message } }
                };
                json.Add("error", res);
            }
            return json;
        }
    }
}
