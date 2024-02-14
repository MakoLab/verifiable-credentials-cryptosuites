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
        public AssertionMethodPurpose(Controller? controller = null, DateTime? date = null, string term = "assertionMethod", int maxTimestampDelta = int.MaxValue)
            : base(term, controller, date, maxTimestampDelta)
        {
        }
    }
}
