﻿using Cryptosuite.Core;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using System.Diagnostics.CodeAnalysis;


namespace ECDsa_Multikey
{
    public class KeyPair
    {
        public string? Id { get; set; }
        public string? Controller { get; set; }
        public required ECPublicKeyParameters PublicKey { get; set; }
        public ECPrivateKeyParameters? SecretKey { get; set; }
        public X9ECParameters? Curve { get; set; }
        public required ECDsaCurveType Algorithm { get; set; }

        public KeyPair()
        {
        }

        // Copy constructor
        [SetsRequiredMembers]
        public KeyPair(KeyPair keyPair)
        {
            Id = keyPair.Id;
            Controller = keyPair.Controller;
            PublicKey = keyPair.PublicKey;
            SecretKey = keyPair.SecretKey;
            Curve = keyPair.Curve;
            Algorithm = keyPair.Algorithm;
        }

        public MultikeyVerificationMethod Export(ExportKeyPairOptions exportOptions = ExportKeyPairOptions.IncludePublicKey | ExportKeyPairOptions.IncludeContext)
        {
            return Serialize.ExportKeyPair(this, exportOptions);
        }

        public string GetPublicKeyMultibase()
        {
            if (PublicKey is null)
            {
                throw new ArgumentException("Key pair does not contain keys.");
            }
            return Serialize.ExtractPublicKeyMultibase(PublicKey, Algorithm);
        }
    }
}
