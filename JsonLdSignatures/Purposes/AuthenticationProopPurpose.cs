using Cryptosuite.Core;
using FluentResults;
using JsonLdExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonLdSignatures.Purposes
{
    public class AuthenticationProopPurpose : ControllerProofPurpose
    {
        private readonly string _challenge;
        private readonly string? _domain;

        public AuthenticationProopPurpose(Controller controller, string challenge, DateTime date, string? domain, string term = "authentication", int maxTimestampDelta = Int32.MaxValue)
            : base(term, controller, date, maxTimestampDelta)
        {
            _challenge = challenge;
            _domain = domain;
        }

        public override Result<ValidationResult> Validate(Proof proof, VerificationMethod verificationMethod, IDocumentLoader documentLoader)
        {
            if (proof.Challenge != _challenge)
            {
                return Result.Fail("The proof's challenge does not match the expected challenge.");
            }
            if (_domain is not null && (proof.Domain is null || !proof.Domain.Contains(_domain)))
            {
                return Result.Fail("The proof's domain does not match the expected domain.");
            }
            return base.Validate(proof, verificationMethod, documentLoader);
        }

        public new Proof Update(Proof proof)
        {
            var updated = base.Update(proof);
            updated.Challenge = _challenge;
            updated.Domain = _domain is not null ? new List<string> { _domain } : null;
            return updated;
        }
    }
}
