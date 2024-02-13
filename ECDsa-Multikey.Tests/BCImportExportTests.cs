using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECDsa_Multikey;

namespace ECDsa_Multikey.Tests
{
    public class BCImportExportTests
    {
        [Fact]
        public void CreateKeyPairFromCompressedPublicKey()
        {
            var publicKeyBase64 = "MDkwEwYHKoZIzj0CAQYIKoZIzj0DAQcDIgADJsDXbCAWoV7dhtu8HT9WxXdmXBd/t082s/N/uWYNN54=";
        }
    }
}
