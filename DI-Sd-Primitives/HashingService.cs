using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using Org.BouncyCastle.Crypto.Digests;

namespace DI_Sd_Primitives
{
    public class HashingService
    {
        public byte[] HashMandatoryNQuads(List<string> nQuads)
        {
            var digest = new Sha256Digest();
            var input = Encoding.UTF8.GetBytes(String.Join("", nQuads));
            digest.BlockUpdate(input, 0, input.Length);
            var output = new byte[digest.GetDigestSize()];
            digest.DoFinal(output, 0);
            return output;
        }
    }
}
