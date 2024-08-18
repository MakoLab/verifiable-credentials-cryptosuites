using FluentResults;

namespace Cryptosuite.Core.Interfaces
{
    public interface IProofPurpose
    {
        public Result<ValidationResult> Validate(Proof proof);
        public Proof Update(Proof proof);
        public bool Match(Proof proof);
    }
}
