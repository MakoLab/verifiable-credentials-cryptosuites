using Cryptosuite.Core;
using ECDsa_Multikey;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ECDsa_Multikey
{
    internal class Serialize
    {
        private static readonly Dictionary<string, byte[]> SpkiPrefixes = new()
        {
            {
                ECDsaCurve.P256,
                new byte[]
                {
                    48, 57, 48, 19, 6, 7, 42, 134, 72, 206,
                    61, 2, 1, 6, 8, 42, 134, 72, 206, 61,
                    3, 1, 7, 3, 34, 0
                }
            },
            {
                ECDsaCurve.P384,
                new byte[]
                {
                    48, 70, 48, 16, 6, 7, 42, 134, 72, 206, 61, 2,
                    1, 6, 5, 43, 129, 4, 0, 34, 3, 50, 0
                }
            },
            {
            ECDsaCurve.P521,
                new byte[]
                {
                    48, 88, 48, 16, 6, 7, 42, 134, 72, 206, 61, 2,
                    1, 6, 5, 43, 129, 4, 0, 35, 3, 68, 0
                }
            }
        };

        private static readonly Dictionary<string, (byte[] secret, byte[] pub)> Pkcs8Prefixes = new()
        {
            {
                ECDsaCurve.P256,
                (
                    new byte[]
                    {
                        48, 103, 2, 1, 0, 48, 19, 6, 7, 42, 134, 72,
                        206, 61, 2, 1, 6, 8, 42, 134, 72, 206, 61, 3,
                        1, 7, 4, 77, 48, 75, 2, 1, 1, 4, 32
                    },
                    new byte[] { 161, 36, 3, 34, 0 }
                )
            },
            {
                ECDsaCurve.P384,
                (
                    new byte[]
                    {
                        48, 129, 132, 2, 1, 0, 48, 16, 6, 7, 42, 134,
                        72, 206, 61, 2, 1, 6, 5, 43, 129, 4, 0, 34,
                        4, 109, 48, 107, 2, 1, 1, 4, 48
                    },
                    new byte[] { 161, 52, 3, 50, 0 }
                )
            },
            {
                ECDsaCurve.P521,
                (
                    new byte[]
                    {
                        48, 129, 170, 2, 1, 0, 48, 16, 6, 7, 42, 134,
                        72, 206, 61, 2, 1, 6, 5, 43, 129, 4, 0, 35,
                        4, 129, 146, 48, 129, 143, 2, 1, 1, 4, 66
                    },
                    new byte[] { 161, 70, 3, 68, 0 }
                )
            }
        };

        internal static KeyPair ImportKeyPair(MultikeyModel multikey)
        {
            return ImportKeyPair(multikey.Id, multikey.Controller, multikey.SecretKeyMultibase, multikey.PublicKeyMultibase);
        }

        internal static KeyPair ImportKeyPair(string id, string controller, string secretKeyMultibase, string publicKeyMultibase)
        {
            if (publicKeyMultibase is null)
            {
                throw new ArgumentException($"The {nameof(publicKeyMultibase)} property is required.");
            }
            if (publicKeyMultibase[0] != Constants.MultibaseBase58Header)
            {
                throw new ArgumentException($"{nameof(publicKeyMultibase)} must be a multibase, base58-encoded string.");
            }
            var publicMultikey = SimpleBase.Base58.Bitcoin.Decode(publicKeyMultibase.AsSpan()[1..]);
            var algorithm = Helpers.GetNamedCurveFromPublicMultikey(publicMultikey);
            var curve = FromString(algorithm);
            var keyPair = new KeyPair
            {
                Id = id,
                Controller = controller
            };
            var spki = ToSpki(publicMultikey);
            keyPair.Keys = ECDsa.Create(curve);
            keyPair.Keys.ImportSubjectPublicKeyInfo(spki, out _);

            if (secretKeyMultibase is not null)
            {
                if (secretKeyMultibase[0] != Constants.MultibaseBase58Header)
                {
                    throw new ArgumentException($"{nameof(secretKeyMultibase)} must be a multibase, base58-encoded string.");
                }
                var secretMultikey = SimpleBase.Base58.Bitcoin.Decode(secretKeyMultibase.AsSpan()[1..]);
                EnsureMultikeyHeadersMatch(publicMultikey, secretMultikey);
                var pkcs8 = ToPkcs8(secretMultikey, publicMultikey);
                keyPair.Keys.ImportPkcs8PrivateKey(pkcs8, out _);
            }
            return keyPair;
        }

        internal static MultikeyModel ExportKeyPair(KeyPair keyPair, bool publicKey, bool secretKey, bool includeContext)
        {
            if (!publicKey && !secretKey)
            {
                throw new ArgumentException("Export requires specifying either 'publicKey' or 'secretKey'.");
            }
            if (keyPair.Keys is null)
            {
                throw new ArgumentException("Key pair does not contain keys.");
            }
            var secretKeySize = Helpers.GetSecretKeySize(keyPair.Keys);
            var multiKey = new MultikeyModel
            {
                Id = keyPair.Id,
                Controller = keyPair.Controller,
                Type = "Multikey",
            };
            if (includeContext)
            {
                multiKey.Contexts = new List<string> { Constants.MultikeyContextV1Url };
            }
            var parameters = keyPair.Keys.ExportParameters(includePrivateParameters: true);
            if (publicKey)
            {
                var publicKeySize = secretKeySize + 1;
                var publicMultikey = new byte[2 + publicKeySize];
                Helpers.SetPublicKeyHeader(keyPair.Keys, publicMultikey);
                var x = parameters.Q.X;
                var y = parameters.Q.Y;
                if (x is null || y is null)
                {
                    throw new Exception("Public key is missing.");
                }
                var even = y[^1] % 2 == 0;
                publicMultikey[2] = (byte)(even ? 0x02 : 0x03);
                x.CopyTo(publicMultikey.AsSpan()[^x.Length..]);
                multiKey.PublicKeyMultibase = Constants.MultibaseBase58Header + SimpleBase.Base58.Bitcoin.Encode(publicMultikey);
            }
            if (secretKey)
            {
                var secretMultikey = new byte[2 + secretKeySize];
                Helpers.SetSecretKeyHeader(keyPair.Keys, secretMultikey);
                var d = parameters.D;
                if (d is null)
                {
                    throw new Exception("Secret key is missing.");
                }
                d.CopyTo(secretMultikey.AsSpan()[^d.Length..]);
                multiKey.SecretKeyMultibase = Constants.MultibaseBase58Header + SimpleBase.Base58.Bitcoin.Encode(secretMultikey);
            }
            return multiKey;
        }

        private static byte[] ToSpki(byte[] publicMultikey)
        {
            var header = SpkiPrefixes[Helpers.GetNamedCurveFromPublicMultikey(publicMultikey)];
            var spki = new byte[header.Length + publicMultikey.Length - 2]; // do not include multikey 2-byte header
            var offset = 0;
            header.CopyTo(spki, offset);
            offset += header.Length;
            publicMultikey.AsSpan()[2..].CopyTo(spki.AsSpan()[offset..]);
            return spki;
        }

        private static byte[] ToPkcs8(byte[] secretMultikey, byte[] publicMultikey)
        {
            var (secret, pub) = Pkcs8Prefixes[Helpers.GetNamedCurveFromPublicMultikey(publicMultikey)];
            var pkcs8 = new byte[secret.Length + secretMultikey.Length - 2 + pub.Length + publicMultikey.Length - 2]; // do not include multikey 2-byte headers
            var offset = 0;
            secret.CopyTo(pkcs8, offset);
            offset += secret.Length;
            secretMultikey.AsSpan()[2..].CopyTo(pkcs8.AsSpan()[offset..]);
            offset += secretMultikey.Length - 2;
            pub.CopyTo(pkcs8.AsSpan()[offset..]);
            offset += pub.Length;
            publicMultikey.AsSpan()[2..].CopyTo(pkcs8.AsSpan()[offset..]);
            return pkcs8;
        }

        private static void EnsureMultikeyHeadersMatch(byte[] publicMultikey, byte[] secretMultikey)
        {
            var publicCurve = Helpers.GetNamedCurveFromPublicMultikey(publicMultikey);
            var secretCurve = Helpers.GetNamedCurveFromSecretMultikey(secretMultikey);
            if (publicCurve != secretCurve)
            {
                throw new Exception($"Public key curve ({publicCurve}) does not match secret key curve ({secretCurve}).");
            }
        }

        private static ECCurve FromString(string curveName)
        {
            return curveName switch
            {
                ECDsaCurve.P256 => ECCurve.NamedCurves.nistP256,
                ECDsaCurve.P384 => ECCurve.NamedCurves.nistP384,
                ECDsaCurve.P521 => ECCurve.NamedCurves.nistP521,
                _ => throw new Exception("Unsupported curve name.")
            };
        }
    }
}
