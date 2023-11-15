using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
using Xunit.Abstractions;

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
            _output.WriteLine(Convert.ToHexString(x));
            _output.WriteLine(Convert.ToHexString(y));
            _output.WriteLine(Convert.ToHexString(secret));
            _output.WriteLine(Convert.ToHexString(pkcs8));
            _output.WriteLine(Convert.ToHexString(d));
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
    }
}