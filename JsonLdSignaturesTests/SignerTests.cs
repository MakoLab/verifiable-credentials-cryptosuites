using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptosuite.Core.Tests
{
    public class SignerTests
    {
        [Fact]
        public void Signer_SignDeterministic_ReturnsValidSignature()
        {
            // Arrange
            var message = "sample";
            var privateKey = "C9AFA9D845BA75166B5C215767B1D6934E50C3DB36E89B127B8A622B120F6721";
            var messageHash = System.Security.Cryptography.SHA256.HashData(Encoding.UTF8.GetBytes(message));
            var signer = new ECDsaSigner(new HMacDsaKCalculator(new Sha256Digest()));
            signer.Init(true, new ECPrivateKeyParameters(new BigInteger(privateKey, 16), new ECDomainParameters(SecNamedCurves.GetByName("secp256r1"))));

            // Act
            var signature = signer.GenerateSignature(messageHash);

            // Assert
            Assert.NotNull(signature);
        }

        [Theory]
        [InlineData("1fffffffffffffffffffffffffffe9ae2ed07577265dff7f94451e061e163c61", "503213f78ca44883f1a3b81653cd265f23c1567a16876913b0c2ac2458492836")]
        [InlineData("000000000000000000000000e95e4a5f737059dc60dfc7ad95b3d8139515620f", "000000000000000000000000bed5af16ea3f6a4f62938c4631eb5af7bdbcdbc3")]
        [InlineData("ffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffffff", "7fffffffffffffffffffffffffffffff5d576e7357a4501ddfe92f46681b20a0")]
        public void BigInterger_ToByteArray_ToBigInteger_ReturnsSameValue(string r, string s)
        {
            var numberR = new BigInteger(r, 16);
            var numberS = new BigInteger(s, 16);
            var bytes = new byte[64];
            var rArray = numberR.ToByteArrayUnsigned();
            var sArray = numberS.ToByteArrayUnsigned();
            rArray.CopyTo(bytes, 32 - rArray.Length);
            sArray.CopyTo(bytes, 64 - sArray.Length);
            var resultR = new BigInteger(1, bytes[..32]);
            var resultS = new BigInteger(1, bytes[32..]);
            Assert.Equal(numberR, resultR);
            Assert.Equal(numberS, resultS);
        }
    }
}
