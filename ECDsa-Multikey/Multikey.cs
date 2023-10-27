using Cryptosuite.Core;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ECDsa_Multikey
{
    public class Multikey
    {
        public ECDsa Algorithm { get; set; }
        public string PublicKeyMultibase { get; set; }
        public string SecretKeyMultibase { get; set; }

        public object Export()
        {
            return Algorithm.ExportSubjectPublicKeyInfo();
        }

        public static KeyPairInterface From(MultikeyModel multikey)
        {
            if (multikey.Type is not null && multikey.Type != "Multikey")
            {
                multikey = Translators.ToMultikey(multikey);
                return CreateKeyPairInterface(multikey);
            }
            multikey.Type ??= "Multikey";
            multikey.Contexts ??= new List<string> { Constants.MultikeyContextV1Url };
            if (multikey.Controller is not null && multikey.Id is null)
            {
                multikey.Id = $"{multikey.Controller}#{multikey.PublicKeyMultibase}";
            }
            AssertMultikey(multikey);
            return CreateKeyPairInterface(multikey);
        }

        private static KeyPairInterface CreateKeyPairInterface(MultikeyModel multikey)
        {
            throw new NotImplementedException();

        }

        private static void AssertMultikey(MultikeyModel key)
        {
            if (key.Type is not null && key.Type != "Multikey")
            {
                throw new Exception("'key' must be a Multikey with type 'Multikey'.");
            }
            if (key.Contexts is null || !key.Contexts.Contains(Constants.MultikeyContextV1Url))
            {
                throw new Exception($"'key' must be a Multikey with context {Constants.MultikeyContextV1Url}");
            }
        }
    }
}
