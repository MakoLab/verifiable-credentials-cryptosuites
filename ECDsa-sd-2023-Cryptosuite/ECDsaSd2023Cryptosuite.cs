using Cryptosuite.Core;
using Cryptosuite.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDsa_sd_2023_Cryptosuite
{
    public class ECDsaSd2023Cryptosuite : ICryptosuite
    {
        public string RequiredAlgorithm { get => "P-256"; }

        public string Name { get => "ecdsa-sd-2023"; }

        public Verifier CreateVerifier(VerificationMethod verificationMethod)
        {
            throw new NotImplementedException();
        }
    }
}
