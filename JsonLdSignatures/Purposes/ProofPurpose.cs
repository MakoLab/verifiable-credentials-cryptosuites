using Cryptosuite.Core;
using Cryptosuite.Core.Interfaces;
using FluentResults;

namespace JsonLdSignatures.Purposes
{
    public class ProofPurpose : IProofPurpose
    {
        private string Term { get; }
        private DateTime? Date { get; }
        private int MaxTimestampDelta { get; }

        public ProofPurpose(string term, DateTime? date, int maxTimestampDelta = Int32.MaxValue)
        {
            Term = term;
            Date = date;
            MaxTimestampDelta = maxTimestampDelta;
        }

        public Result<ValidationResult> Validate(Proof proof)
        {
            try
            {
                if (MaxTimestampDelta != Int32.MaxValue)
                {
                    var expected = (Date ?? DateTimeOffset.UtcNow).ToUnixTimeMilliseconds();
                    var delta = MaxTimestampDelta * 1000;
                    var created = proof.Created?.TimeOfDay.TotalMilliseconds;
                    if (created <= expected - delta || created >= expected + delta)
                    {
                        return Result.Fail("The proof's created timestamp is out of range.");
                    }
                }
                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }
        }

        public Proof Update(Proof proof)
        {
            proof.ProofPurpose = Term;
            return proof;
        }

        public bool Match(Proof proof)
        {
            return proof.ProofPurpose == Term;
        }
    }
}
