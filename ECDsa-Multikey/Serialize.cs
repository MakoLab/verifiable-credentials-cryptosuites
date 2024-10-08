using Cryptosuite.Core;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using SimpleBase;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("ECDsa_Multikey.Tests")]
namespace ECDsa_Multikey
{
    [Flags]
    public enum ExportKeyPairOptions
    {
        None = 0,
        IncludePublicKey = 1,
        IncludeSecretKey = 2,
        IncludeContext = 4
    }
    internal class Serialize
    {
        private static readonly Dictionary<ECDsaCurveType, byte[]> SpkiPrefixes = new()
        {
            {
                ECDsaCurveType.P256,
                new byte[]
                {
                    48, 57, 48, 19, 6, 7, 42, 134, 72, 206,
                    61, 2, 1, 6, 8, 42, 134, 72, 206, 61,
                    3, 1, 7, 3, 34, 0
                }
            },
            {
                ECDsaCurveType.P384,
                new byte[]
                {
                    48, 70, 48, 16, 6, 7, 42, 134, 72, 206, 61, 2,
                    1, 6, 5, 43, 129, 4, 0, 34, 3, 50, 0
                }
            },
            {
            ECDsaCurveType.P521,
                new byte[]
                {
                    48, 88, 48, 16, 6, 7, 42, 134, 72, 206, 61, 2,
                    1, 6, 5, 43, 129, 4, 0, 35, 3, 68, 0
                }
            }
        };

        private static readonly Dictionary<ECDsaCurveType, (byte[] secret, byte[] pub)> Pkcs8Prefixes = new()
        {
            {
                ECDsaCurveType.P256,
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
                ECDsaCurveType.P384,
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
                ECDsaCurveType.P521,
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

        internal static KeyPair ImportKeyPair(MultikeyVerificationMethod multikey)
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
            var publicMultikey = Base58.Bitcoin.Decode(publicKeyMultibase.AsSpan()[1..]);
            var curveType = Helpers.GetNamedCurveFromPublicMultikey(publicMultikey);
            var curve = FromCurveType(curveType);
            var keyPair = new KeyPair
            {
                Id = id,
                Controller = controller,
                Curve = curve,
                Algorithm = curveType,
                PublicKey = PublicKeyFactory.CreateKey(ToSpki(publicMultikey)) as ECPublicKeyParameters ?? throw new InvalidOperationException("Failed to create public key."),
            };
            
            if (secretKeyMultibase is not null)
            {
                if (secretKeyMultibase[0] != Constants.MultibaseBase58Header)
                {
                    throw new ArgumentException($"{nameof(secretKeyMultibase)} must be a multibase, base58-encoded string.");
                }
                var secretMultikey = Base58.Bitcoin.Decode(secretKeyMultibase.AsSpan()[1..]);
                EnsureMultikeyHeadersMatch(publicMultikey, secretMultikey);
                var pkcs8 = ToPkcs8(secretMultikey, publicMultikey);
                keyPair.SecretKey = PrivateKeyFactory.CreateKey(pkcs8) as ECPrivateKeyParameters;
            }
            return keyPair;
        }

        internal static MultikeyVerificationMethod ExportKeyPair(KeyPair keyPair, ExportKeyPairOptions exportOptions)
        {
            if (exportOptions == ExportKeyPairOptions.None)
            {
                throw new ArgumentException("Export requires specifying either 'publicKey' or 'secretKey'.");
            }
            if (keyPair.Id is null || keyPair.Controller is null)
            {
                throw new ArgumentException("Key pair does not contain an identifier or controller.");
            }
            var multiKey = new MultikeyVerificationMethod { Id = keyPair.Id, Controller = keyPair.Controller };
            if (exportOptions.HasFlag(ExportKeyPairOptions.IncludeContext))
            {
                multiKey.Context = new JValue(Constants.MultikeyContextV1Url);
            }
            if (exportOptions.HasFlag(ExportKeyPairOptions.IncludePublicKey))
            {
                if (keyPair.PublicKey is null)
                {
                    throw new ArgumentException("Key pair does not contain a public key.");
                }
                multiKey.PublicKeyMultibase = ExtractPublicKeyMultibase(keyPair.PublicKey, keyPair.Algorithm);
            }
            if (exportOptions.HasFlag(ExportKeyPairOptions.IncludeSecretKey))
            {
                if (keyPair.SecretKey is null)
                {
                    throw new ArgumentException("Key pair does not contain a secret key.");
                }
                multiKey.SecretKeyMultibase = ExtractPrivateKeyMultibase(keyPair.SecretKey, keyPair.Algorithm);
            }
            return multiKey;
        }

        /// <summary>
        /// Extracts the private key from the key parameter.
        /// </summary>
        /// <param name="secretKey"></param>
        /// <param name="curveType"></param>
        /// <returns>Private key in multibase format.</returns>
        /// <exception cref="Exception"></exception>
        internal static string ExtractPrivateKeyMultibase(ECPrivateKeyParameters secretKey, ECDsaCurveType curveType)
        {
            var secretKeySize = Helpers.GetSecretKeySize(curveType);
            var secretMultikey = new byte[2 + secretKeySize];
            Helpers.SetSecretKeyHeader(curveType, secretMultikey);

            var d = secretKey.D.ToByteArrayUnsigned() ?? throw new Exception("Secret key is missing.");
            d.CopyTo(secretMultikey.AsSpan()[^d.Length..]);
            return Constants.MultibaseBase58Header + Base58.Bitcoin.Encode(secretMultikey);
        }

        /// <summary>
        /// Extracts the public key from the key pair.
        /// </summary>
        /// <param name="keyPair"></param>
        /// <param name="algorithm"></param>
        /// <returns>Public key in multibase format.</returns>
        /// <exception cref="Exception"></exception>
        internal static string ExtractPublicKeyMultibase(ECPublicKeyParameters publicKey, ECDsaCurveType algorithm)
        {
            var secretKeySize = Helpers.GetSecretKeySize(algorithm);
            var publicKeySize = secretKeySize + 1;
            var publicMultikey = new byte[2 + publicKeySize].AsSpan();
            Helpers.SetPublicKeyHeader(algorithm, publicMultikey);
            var x = publicKey.Q.XCoord.GetEncoded();
            var y = publicKey.Q.YCoord.GetEncoded();
            if (x is null || y is null)
            {
                throw new Exception("Public key is missing.");
            }
            var even = y[^1] % 2 == 0;
            publicMultikey[2] = (byte)(even ? 0x02 : 0x03);
            x.CopyTo(publicMultikey[^x.Length..]);
            return Constants.MultibaseBase58Header + Base58.Bitcoin.Encode(publicMultikey);
        }

        private static byte[] ToSpki(ReadOnlySpan<byte> publicMultikey)
        {
            var header = SpkiPrefixes[Helpers.GetNamedCurveFromPublicMultikey(publicMultikey)].AsSpan();
            byte[] spki = [.. header, .. publicMultikey[2..]];
            return spki;
        }

        private static byte[] ToPkcs8(ReadOnlySpan<byte> secretMultikey, ReadOnlySpan<byte> publicMultikey)
        {
            var (secret, pub) = Pkcs8Prefixes[Helpers.GetNamedCurveFromPublicMultikey(publicMultikey)];
            var secretSpan = secret.AsSpan();
            var pubSpan = pub.AsSpan();
            byte[] pkcs8 = [.. secretSpan, .. secretMultikey[2..], .. pubSpan, .. publicMultikey[2..]];
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

        private static X9ECParameters FromCurveType(ECDsaCurveType curve)
        {
            return curve switch
            {
                ECDsaCurveType.P256 => ECNamedCurveTable.GetByName("P-256"),
                ECDsaCurveType.P384 => ECNamedCurveTable.GetByName("P-384"),
                ECDsaCurveType.P521 => ECNamedCurveTable.GetByName("P-521"),
                _ => throw new Exception("Unsupported curve name.")
            };
        }
    }
}
