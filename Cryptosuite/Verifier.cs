using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptosuite.Core
{
    public class Verifier
    {
        public string Algorithm { get; set; }

        public bool Verify(object verifyData, byte[] signature)
        {
            throw new NotImplementedException();
        }
    }
}
