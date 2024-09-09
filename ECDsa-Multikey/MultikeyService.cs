using Cryptosuite.Core;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using SimpleBase;

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
                PublicKey = keys.Public as ECPublicKeyParameters ?? throw new InvalidOperationException("Failed to create public key."),
                SecretKey = keys.Private as ECPrivateKeyParameters
            };
            var kpi = CreateKeyPairInterface(keyPair);
            var publicKeyMultibase = kpi.KeyPair.GetPublicKeyMultibase();
            if (controller is not null && id is null)
            {
                id = $"{controller}#{publicKeyMultibase}";
            }
            kpi.KeyPair.Id = id;
            kpi.KeyPair.Controller = controller;
            return kpi;
        }

        public static KeyPairInterface From(MultikeyVerificationMethod multikey)
        {
            if (multikey.Type is not null && !multikey.Type.Equals("multikey", StringComparison.InvariantCultureIgnoreCase))
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
            if (keyPair.PublicKey is null)
            {
                throw new ArgumentException("Key pair does not contain public key.");
            }
            var kpi = new KeyPairInterface
            {
                KeyPair = new KeyPair(keyPair),
                Verifier = new Verifier(keyPair.Id, keyPair.PublicKey, keyPair.Algorithm),
            };
            if (keyPair.SecretKey is not null)
            {
                kpi.Signer = new Signer(keyPair.Id, keyPair.SecretKey, keyPair.Algorithm);
            }
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
