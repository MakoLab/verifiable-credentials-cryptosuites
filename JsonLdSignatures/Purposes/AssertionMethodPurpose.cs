using Cryptosuite.Core;

namespace JsonLdSignatures.Purposes
{
    public class AssertionMethodPurpose : ControllerProofPurpose
    {
        public AssertionMethodPurpose(Controller? controller = null, DateTime? date = null, string term = "assertionMethod", int maxTimestampDelta = int.MaxValue)
            : base(term, controller, date, maxTimestampDelta)
        {
        }
    }
}
