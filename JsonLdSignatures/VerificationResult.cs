using Cryptosuite.Core;
using FluentResults;
using JsonLdSignatures.Purposes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonLdSignatures
{
    public class VerificationResult
    {
        public Proof? Proof { get; set; }
        public VerificationMethod? VerificationMethod { get; set; }
        public IList<Result> PurposeValidation { get; set; } = new List<Result>();
    }
}
