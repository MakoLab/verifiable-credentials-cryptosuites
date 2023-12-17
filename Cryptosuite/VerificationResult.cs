using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptosuite.Core
{
    public class VerificationResult
    {
        public Proof? Proof { get; set; }
        public VerificationMethod? VerificationMethod { get; set; }
        public List<Result<ValidationResult>> PurposeValidation { get; set; } = new();
    }
}
