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
        public static KeyPairInterface Generate(string id, string controller, string curve)
        {
            var eccurve = ECDsaCurve.ToECCurve(curve);
            var keypair = new KeyPair
            {
                Keys = ECDsa.Create(eccurve),
            };
            var kpi = CreateKeyPairInterface(keypair);
            var exported = kpi.Export(publicKey: true);
            var publicKeyMultibase = exported.PublicKeyMultibase;
            if (controller is not null && id is null)
            {
                id = $"{controller}#{publicKeyMultibase}";
            }
            kpi.Id = id;
            kpi.Controller = controller;
            return kpi;
        }

        public static KeyPairInterface From(MultikeyModel multikey)
        {
            if (multikey.Type is not null && multikey.Type != "Multikey")
            {
                multikey = Translators.ToMultikey(multikey);
                return CreateKeyPairInterface(multikey);
            }
            multikey.Type ??= "Multikey";
            multikey.Context ??= new JValue(Constants.MultikeyContextV1Url);
            if (multikey.Controller is not null && multikey.Id is null)
            {
                multikey.Id = $"{multikey.Controller}#{multikey.PublicKeyMultibase}";
            }
            AssertMultikey(multikey);
            return CreateKeyPairInterface(multikey);
        }

        private static KeyPairInterface CreateKeyPairInterface(MultikeyModel multikey)
        {
            var keypair = Serialize.ImportKeyPair(multikey);
            return CreateKeyPairInterface(keypair);
        }

        private static KeyPairInterface CreateKeyPairInterface(KeyPair keyPair)
        {
            if (keyPair.Keys is null)
            {
                throw new ArgumentException("Key pair does not contain keys.");
            }
            var multi = Serialize.ExportKeyPair(keyPair, true, true, true);
            var kpi = new KeyPairInterface
            {
                Id = keyPair.Id,
                Controller = keyPair.Controller,
                Keys = keyPair.Keys,
                PublicKeyMultibase = multi.PublicKeyMultibase,
                SecretKeyMultibase = multi.SecretKeyMultibase,
                Verifier = new Verifier(keyPair.Id, keyPair.Keys),
                Signer = new Signer(keyPair.Id, keyPair.Keys),
            };
            return kpi;
        }

        private static void AssertMultikey(MultikeyModel key)
        {
            if (key.Type is not null && key.Type != "Multikey")
            {
                throw new Exception("'key' must be a Multikey with type 'Multikey'.");
            }
            AssertContext(key.Context);
        }

        private static void AssertContext(JToken? context)
        {
            if (context is null)
            {
                throw new Exception("'key' must be a Multikey with a context.");
            }
            if (context is JValue v)
            {
                if (v.Value?.ToString() == Constants.MultikeyContextV1Url)
                {
                    return;
                }
            }
            if (context is JArray a)
            {
                if (a.Select(i => i.ToString()).Contains(Constants.MultikeyContextV1Url))
                {
                    return;
                }
            }
            throw new Exception($"'key' must be a Multikey with a context of '{Constants.MultikeyContextV1Url}'.");
        }

    }
}
