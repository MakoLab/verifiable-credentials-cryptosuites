using Cryptosuite.Core;
using Cryptosuite.Core.ControllerDocuments;
using ECDsa_Multikey;
using Newtonsoft.Json.Linq;

namespace TestWebAPI
{
    public class MockData
    {
        public static string ControllerId { get; set; } = "https://db.makolab.pro/issuers";
        public static string PublicKeyMultibase { get; set; } = "zDnaekGZTbQBerwcehBSXLqAg6s55hVEBms1zFy89VHXtJSa1";
        public static string SecretKeyMultibase { get; set; } = "z42tqZ5smVag3DtDhjY9YfVwTMyVHW6SCHJi2ZMrD23DGYS3";
        public static string VerificationMethodId { get; set; } = $"{ControllerId}/{PublicKeyMultibase}";

        public static string GetControllerDocument()
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

        public static string GetVerificationMethodDocument(string id)
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
