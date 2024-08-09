using Cryptosuite.Core;
using Cryptosuite.Core.ControllerDocuments;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDsa_Multikey
{
    internal class Translators
    {
        private static readonly string[] ValidECDsaTypes = new string[] { Constants.ECDsa2019Secp256KeyType, Constants.ECDsa2019Secp384KeyType, Constants.ECDsa2019Secp521KeyType };
        public static MultikeyVerificationMethod ToMultikey(MultikeyVerificationMethod keyPair)
        {
            if (!ValidECDsaTypes.Contains(keyPair.Type))
            {
                throw new Exception($"Unsupported key type {keyPair.Type}");
            }
            keyPair.Context ??= new JValue(Constants.ECDsa2019SuiteContextV1Url);
            if (!IncludesContext(keyPair, Constants.ECDsa2019SuiteContextV1Url))
            {
                throw new Exception($"Context not supported {keyPair.Context}");
            }
            return new MultikeyVerificationMethod(keyPair.Id, keyPair.Controller)
            {
                Context = new JValue(Constants.MultikeyContextV1Url),
                PublicKeyMultibase = keyPair.PublicKeyMultibase,
                SecretKeyMultibase = keyPair.SecretKeyMultibase
            };
        }

        private static bool IncludesContext(MultikeyVerificationMethod keyPair, string contextUrl)
        {
            return keyPair.Context?.Contains(contextUrl) ?? false;
        }
    }
}
