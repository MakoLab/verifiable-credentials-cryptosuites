using Cryptosuite.Core;
using ECDsa_Multikey;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("ECDsa_Multikey.Tests")]
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

        internal static KeyPair ImportKeyPair(string? id, string? controller, string? secretKeyMultibase, string? publicKeyMultibase)
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
                Controller = controller,
                Curve = curve,
                Algorithm = algorithm,
            };
            var publicKey = PublicKeyFactory.CreateKey(ToSpki(publicMultikey));
            AsymmetricKeyParameter? privateKey = null;

            if (secretKeyMultibase is not null)
            {
                if (secretKeyMultibase[0] != Constants.MultibaseBase58Header)
                {
                    throw new ArgumentException($"{nameof(secretKeyMultibase)} must be a multibase, base58-encoded string.");
                }
                var secretMultikey = SimpleBase.Base58.Bitcoin.Decode(secretKeyMultibase.AsSpan()[1..]);
                EnsureMultikeyHeadersMatch(publicMultikey, secretMultikey);
                var pkcs8 = ToPkcs8(secretMultikey, publicMultikey);
                privateKey = PrivateKeyFactory.CreateKey(pkcs8);
            }
            keyPair.Keys = new AsymmetricCipherKeyPair(publicKey, privateKey);
            return keyPair;
        }

        internal static MultikeyModel ExportKeyPair(KeyPair keyPair, bool includePublicKey, bool includeSecretKey, bool includeContext)
        {
            if (!includePublicKey && !includeSecretKey)
            {
                throw new ArgumentException("Export requires specifying either 'publicKey' or 'secretKey'.");
            }
            if (keyPair.Keys is null)
            {
                throw new ArgumentException("Key pair does not contain keys.");
            }
            var secretKeySize = Helpers.GetSecretKeySize(keyPair.Algorithm);
            var multiKey = new MultikeyModel
            {
                Id = keyPair.Id,
                Controller = keyPair.Controller,
                Type = "Multikey",
            };
            if (includeContext)
            {
                multiKey.Context = new JValue(Constants.MultikeyContextV1Url);
            }
            if (includePublicKey)
            {
                var publicKeySize = secretKeySize + 1;
                var publicMultikey = new byte[2 + publicKeySize];
                var publicKey = (ECPublicKeyParameters)keyPair.Keys.Public;
                Helpers.SetPublicKeyHeader(keyPair.Algorithm, publicMultikey);
                var x = publicKey.Q.XCoord.GetEncoded();
                var y = publicKey.Q.YCoord.GetEncoded();
                if (x is null || y is null)
                {
                    throw new Exception("Public key is missing.");
                }
                var even = y[^1] % 2 == 0;
                publicMultikey[2] = (byte)(even ? 0x02 : 0x03);
                x.CopyTo(publicMultikey.AsSpan()[^x.Length..]);
                multiKey.PublicKeyMultibase = Constants.MultibaseBase58Header + SimpleBase.Base58.Bitcoin.Encode(publicMultikey);
            }
            if (includeSecretKey)
            {
                var secretMultikey = new byte[2 + secretKeySize];
                Helpers.SetSecretKeyHeader(keyPair.Algorithm, secretMultikey);
                var privateKey = (ECPrivateKeyParameters)keyPair.Keys.Private;
                var d = privateKey.D.ToByteArrayUnsigned();
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
            var spki = new byte[header.Length + publicMultikey.AsSpan()[2..].Length]; // do not include multikey 2-byte header
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

        //private static byte[] DecompressKey(Span<byte> key, ECCurve curve)
        //{
        //    if (curve.Prime is null || curve.A is null || curve.B is null)
        //    {
        //        throw new Exception("Curve parameters are missing.");
        //    }
        //    var p = new BigInteger(curve.Prime);
        //    var x = new BigInteger(key[1..]);
        //    var a = new BigInteger(curve.A);
        //    var b = new BigInteger(curve.B);
        //    var ysq = (BigInteger.ModPow(x, 3, p) + a * x + b) % p;
        //    var y = BigInteger.ModPow(ysq, (p + 1) / 4, p);
        //    if (y % 2 != key[0] % 2)
        //    {
        //        y = p - y;
        //    }
        //    var result = new byte[key.Length * 2 - 1];
        //    result[0] = 0x04;
        //    x.ToByteArray(isBigEndian: false, isUnsigned: true).CopyTo(result.AsSpan()[1..]);
        //    y.ToByteArray(isBigEndian: false, isUnsigned: true).CopyTo(result.AsSpan()[(1 + x.ToByteArray().Length)..]);
        //    return result;
        //}

        private static void EnsureMultikeyHeadersMatch(byte[] publicMultikey, byte[] secretMultikey)
        {
            var publicCurve = Helpers.GetNamedCurveFromPublicMultikey(publicMultikey);
            var secretCurve = Helpers.GetNamedCurveFromSecretMultikey(secretMultikey);
            if (publicCurve != secretCurve)
            {
                throw new Exception($"Public key curve ({publicCurve}) does not match secret key curve ({secretCurve}).");
            }
        }

        private static X9ECParameters FromString(string curveName)
        {
            return curveName switch
            {
                ECDsaCurve.P256 => ECNamedCurveTable.GetByName("P-256"),
                ECDsaCurve.P384 => ECNamedCurveTable.GetByName("P-384"),
                ECDsaCurve.P521 => ECNamedCurveTable.GetByName("P-521"),
                _ => throw new Exception("Unsupported curve name.")
            };
        }
    }
}
