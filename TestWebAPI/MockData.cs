using Cryptosuite.Core;
using Cryptosuite.Core.ControllerDocuments;
using ECDsa_Multikey;
using Newtonsoft.Json.Linq;

namespace TestWebAPI
{
    public class MockData
    {
        public static string ControllerId { get; set; } = "https://localhost:40443/issuers";
        public static string PublicKeyMultibase { get; set; } = "zDnaekGZTbQBerwcehBSXLqAg6s55hVEBms1zFy89VHXtJSa9";
        public static string SecretKeyMultibase { get; set; } = "z42tqZ5smVag3DtDhjY9YfVwTMyVHW6SCHJi2ZMrD23DGYS3";
        public static string VerificationMethodId { get; set; } = $"{ControllerId}/{PublicKeyMultibase}";

        public static string GetControllerDocument()
        {
            var cd = new ControllerDocument(ControllerId)
            {
                Context = new JArray(
                    Contexts.CredentialsContextV2Url
                    ),
                VerificationMethod = [new MultikeyVerificationMethod(VerificationMethodId, ControllerId)
                {
                    PublicKeyMultibase = PublicKeyMultibase,
                }],
                AssertionMethod = [VerificationMethodId],
            };
            return JObject.FromObject(cd).ToString();
        }

        public static string GetVerificationMethodDocument(string id)
        {
            if (id != PublicKeyMultibase)
            {
                return "{}";
            }
            var vm = new MultikeyVerificationMethod(VerificationMethodId, ControllerId)
            {
                Context = new JArray(
                    Contexts.CredentialsContextV2Url
                    ),
                PublicKeyMultibase = PublicKeyMultibase,
            };
            return JObject.FromObject(vm).ToString();
        }
    }
}
