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

        public IEnumerable<Result> Verify(JObject document, LinkedDataSignature[] suites, ProofPurpose purpose, IDocumentLoader documentLoader)
        {
            return _proofSetService.Verify(document, suites, purpose, documentLoader);
        }
    }
}
