using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptosuite.Core.Interfaces
{
    public interface ICreateVerifier
    {
        Verifier CreateVerifier(VerificationMethod verificationMethod);
    }
}
