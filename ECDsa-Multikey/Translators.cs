using Cryptosuite.Core;
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
        public static MultikeyModel ToMultikey(MultikeyModel keyPair)
        {
            if (!ValidECDsaTypes.Contains(keyPair.Type))
            {
                throw new Exception($"Unsupported key type {keyPair.Type}");
            }
            keyPair.Contexts ??= new List<string> { Constants.ECDsa2019SuiteContextV1Url };
            if (!IncludesContext(keyPair, Constants.ECDsa2019SuiteContextV1Url))
            {
                throw new Exception($"Context not supported {keyPair.Contexts}");
            }
            return new MultikeyModel
            {
                Contexts = new List<string> { Constants.MultikeyContextV1Url },
                Type = "Multikey",
                Id = keyPair.Id,
                Controller = keyPair.Controller,
                PublicKeyMultibase = keyPair.PublicKeyMultibase,
                SecretKeyMultibase = keyPair.SecretKeyMultibase
            };
        }

        private static bool IncludesContext(MultikeyModel keyPair, string contextUrl)
        {
            return keyPair.Contexts?.Contains(contextUrl) ?? false;
        }
    }
}
