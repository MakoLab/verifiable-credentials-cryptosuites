using Cryptosuite.Core.ControllerDocuments;
using FluentResults;

namespace Cryptosuite.Core
{
    public class VerificationResult
    {
        public Proof? Proof { get; set; }
        public VerificationMethod? VerificationMethod { get; set; }
        public List<Result<ValidationResult>> PurposeValidation { get; set; } = new();
    }
}
