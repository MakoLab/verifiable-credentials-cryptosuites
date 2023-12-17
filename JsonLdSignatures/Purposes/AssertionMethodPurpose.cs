using Cryptosuite.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonLdSignatures.Purposes
{
    public class AssertionMethodPurpose : ControllerProofPurpose
    {
        public AssertionMethodPurpose(Controller? controller, DateTime date, string term = "assertion", int maxTimestampDelta = int.MaxValue)
            : base(term, controller, date, maxTimestampDelta)
        {
        }
    }
}
