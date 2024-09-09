using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
using Xunit.Abstractions;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.X509;
using System.Text;

namespace ECDsa_Multikey.Tests
{
    public class UnitTest1
    {
        private readonly ITestOutputHelper _output;

        public UnitTest1(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void Test1()
        {
            var ecdsa = ECDsa.Create(ECCurve.NamedCurves.nistP256);
            var pub = ecdsa.ExportSubjectPublicKeyInfo();
            var param = ecdsa.ExportParameters(true);
            var (x, y) = (param.Q.X, param.Q.Y);
            var secret = ecdsa.ExportECPrivateKey();
            var pkcs8 = ecdsa.ExportPkcs8PrivateKey();
            var d = ecdsa.ExportParameters(true).D;
            _output.WriteLine(Convert.ToHexString(pub));
            _output.WriteLine(Convert.ToHexString(x!));
            _output.WriteLine(Convert.ToHexString(y!));
            _output.WriteLine(Convert.ToHexString(secret));
            _output.WriteLine(Convert.ToHexString(pkcs8));
            _output.WriteLine(Convert.ToHexString(d!));
            _output.WriteLine(param.Curve.Oid.FriendlyName);
        }

        [Fact]
        public void Test2()
        {
            var token = JToken.Parse("{}");
            Assert.NotNull(token);
        }

        [Fact]
        public void Test3()
        {
            var uri = new Uri("https://schema.org#AlumniCredential");
            var s = uri.OriginalString;
            Assert.NotNull(uri);
        }

        [Fact]
        public void Test4()
        {
            var uri = new Uri("did:key:0f343zi09gd0sgsgsdj0v0s9v#0f343zi09gd0sgsgsdj0v0s9v");
            var scheme = uri.Scheme;
            Assert.NotNull(uri);
            Assert.NotNull(scheme);
        }

        [Fact]
        public void PublicOnlyKeypair()
        {
            var messageStr = "Hello, world!";
            var message = Encoding.UTF8.GetBytes(messageStr);
            var generator = new ECKeyPairGenerator("ECDSA");
            var curve = ECNamedCurveTable.GetByName("secp256k1");
            var domainParams = new ECDomainParameters(curve.Curve, curve.G, curve.N, curve.H, curve.GetSeed());
            var keyParams = new ECKeyGenerationParameters(domainParams, new SecureRandom());
            generator.Init(keyParams);
            var keypair = generator.GenerateKeyPair();
            var pkcs = PrivateKeyInfoFactory.CreatePrivateKeyInfo(keypair.Private).GetDerEncoded();
            var privateKey = PrivateKeyFactory.CreateKey(pkcs);
            var signer = new ECDsaSigner();
            signer.Init(true, privateKey);
            var signature = signer.GenerateSignature(message);

            var spki = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(keypair.Public).GetDerEncoded();
            var publicKey = PublicKeyFactory.CreateKey(spki);
            var verifier = new ECDsaSigner();
            verifier.Init(false, publicKey);
            var verification = verifier.VerifySignature(message, signature[0], signature[1]);
            Assert.True(verification);
        }
    }
}