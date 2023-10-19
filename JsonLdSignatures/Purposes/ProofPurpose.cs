using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonLdSignatures.Purposes
{
    public class ProofPurpose
    {
        private string Term { get; }
        private DateTime Date { get; }
        private int MaxTimestampDelta { get; }

        public ProofPurpose(string term, DateTime date, int maxTimestampDelta = Int32.MaxValue)
        {
            Term = term;
            Date = date;
            MaxTimestampDelta = maxTimestampDelta;
        }

        public Result Validate(Proof proof)
        {
            if (MaxTimestampDelta != Int32.MaxValue)
            {
                var expected = Date.TimeOfDay.TotalMilliseconds;
                var delta = MaxTimestampDelta * 1000;
                var created = proof.Created?.TimeOfDay.TotalMilliseconds;
                if (created <= expected - delta || created >= expected + delta)
                {
                    return Result.Fail("The proof's created timestamp is out of range.");
                }
            }
            return Result.Ok();
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
