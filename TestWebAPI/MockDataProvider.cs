using Cryptosuite.Core;
using Cryptosuite.Core.ControllerDocuments;
using ECDsa_Multikey;
using Newtonsoft.Json.Linq;

namespace TestWebAPI
{
    public class MockDataProvider
    {
        private readonly IConfiguration _config;

        public MockDataProvider(IConfiguration config)
        {
            _config = config;
            var baseUrl = _config["BaseUrl"] ?? throw new ArgumentNullException("BaseUrl is not set in appsettings.json");
            ControllerId = $"{baseUrl}/issuers/{PublicKeyMultibase}";
            VerificationMethodId = $"{ControllerId}/{PublicKeyMultibase}";
        }

        public string ControllerId { get; private set; }
        public string VerificationMethodId { get; private set; }
        public string PublicKeyMultibase { get; set; } = "zDnaeipTBN8tgRmkjZWaQSBFj4Ub3ywWP6vAsgGET922nkvZz";
        public string SecretKeyMultibase { get; set; } = "z42tnbVkUeRLg26SD6j5oAG4XHNgQpVeJbA7p4zfhH75JyxC";

        public string GetControllerDocument()
        {
            var cd = new ControllerDocument(ControllerId)
            {
                Context = new JArray(
                    Contexts.CredentialsContextV2Url
                    ),
                VerificationMethod = [new MultikeyVerificationMethod()
                {
                    Id = VerificationMethodId,
                    Controller = ControllerId,
                    PublicKeyMultibase = PublicKeyMultibase,
                }],
                AssertionMethod = [VerificationMethodId],
            };
            return JObject.FromObject(cd).ToString();
        }

        public string GetVerificationMethodDocument(string id)
        {
            if (id != PublicKeyMultibase)
            {
                return "{}";
            }
            var vm = new MultikeyVerificationMethod()
            {
                Context = new JArray(
                    Contexts.CredentialsContextV2Url
                    ),
                Id = VerificationMethodId,
                Controller = ControllerId,
                PublicKeyMultibase = PublicKeyMultibase,
            };
            return JObject.FromObject(vm).ToString();
        }
    }
}
