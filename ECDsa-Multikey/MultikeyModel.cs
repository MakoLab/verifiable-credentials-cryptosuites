using Cryptosuite.Core;
using Cryptosuite.Core.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDsa_Multikey
{
    public class MultikeyModel : VerificationMethod
    {
        public DateTime? Revoked { get; set; }
        public DateTime? Expires { get; set; }
        public string? PublicKeyMultibase { get; set; }
        public string? SecretKeyMultibase { get; set; }
    }
}
