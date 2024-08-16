using Cryptosuite.Core;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using SimpleBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ECDsa_Multikey
{
    public class MultikeyService
    {
        public static KeyPairInterface Generate(string? id, string? controller, string curveName)
        {
            // generate bouncy castle ecdsa key pair
            var curve = ECNamedCurveTable.GetByName(curveName);
            var domainParams = new ECDomainParameters(curve.Curve, curve.G, curve.N, curve.H, curve.GetSeed());
            var keyParams = new ECKeyGenerationParameters(domainParams, new SecureRandom());
            var generator = new ECKeyPairGenerator("ECDSA");
            generator.Init(keyParams);
            var keys = generator.GenerateKeyPair();
            var keyPair = new KeyPair
            {
                Id = id,
                Controller = controller,
                Algorithm = curveName,
                Keys = keys,
            };
            var kpi = CreateKeyPairInterface(keyPair);
            var exported = kpi.Export(includePublicKey: true);
            var publicKeyMultibase = exported.PublicKeyMultibase;
            if (controller is not null && id is null)
            {
                id = $"{controller}#{publicKeyMultibase}";
            }
            kpi.Id = id;
            kpi.Controller = controller;
            return kpi;
        }

        public static KeyPairInterface From(MultikeyVerificationMethod multikey)
        {
            if (multikey.Type is not null && multikey.Type.ToLower() != "multikey")
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

        private static KeyPairInterface CreateKeyPairInterface(MultikeyVerificationMethod multikey)
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
            var kpi = new KeyPairInterface
            {
                Id = keyPair.Id,
                Controller = keyPair.Controller,
                Keys = keyPair.Keys,
                Algorithm = keyPair.Algorithm,
                Verifier = new Verifier(keyPair.Id, keyPair.Keys, keyPair.Algorithm),
                Signer = new Signer(keyPair.Id, keyPair.Keys, keyPair.Algorithm),
            };
            return kpi;
        }

        public static byte[] ToByteArray(string multibase)
        {
            return Base58.Bitcoin.Decode(multibase.AsSpan()[1..]);
        }

        public static string ToMultibaseString(byte[] bytes)
        {
            return $"{Constants.MultibaseBase58Header}{Base58.Bitcoin.Encode(bytes)}";
        }

        private static void AssertMultikey(MultikeyVerificationMethod key)
        {
            if (key.Type is not null && key.Type.ToLower() != "multikey")
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
