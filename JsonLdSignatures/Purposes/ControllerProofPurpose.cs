using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cryptosuite.Core;
using JsonLdExtensions;

namespace JsonLdSignatures.Purposes
{
    public class ControllerProofPurpose : ProofPurpose
    {
        const string DidContextV1 = "https://www.w3.org/ns/did/v1";
        private readonly string[] DidVrTerms = new string[]
        {
            "assertionMethod",
            "authentication",
            "capabilityInvocation",
            "capabilityDelegation",
            "keyAgreement",
            "verificationMethod"
        };
        private readonly Controller? _controller;
        private readonly bool _termDefinedByDIDContext;

        public ControllerProofPurpose(string term, Controller? controller, DateTime date, int maxTimestampDelta = int.MaxValue)
            : base(term, date, maxTimestampDelta)
        {
            _controller = controller;
            _termDefinedByDIDContext = DidVrTerms.Contains(term);
        }

        public virtual Result Validate(Proof proof, VerificationMethod verificationMethod, IDocumentLoader documentLoader)
        {
            var result = base.Validate(proof);
            if (result.IsFailed) return result;

            if (_controller is null)
            {
                return Result.Fail("The controller is not defined.");
            }
            throw new NotImplementedException();
        }
    }

    
}
