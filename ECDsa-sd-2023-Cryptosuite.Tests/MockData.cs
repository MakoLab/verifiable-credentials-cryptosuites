﻿using ECDsa_Multikey;
using Newtonsoft.Json.Linq;

namespace ECDsa_sd_2023_Cryptosuite.Tests
{
    internal class MockData
    {
        public static string ControllerId { get; set; } = "http://localhost:40443/issuers";
        public static string PublicKeyMultibase { get; set; } = "zDnaekGZTbQBerwcehBSXLqAg6s55hVEBms1zFy89VHXtJSa9";
        public static string SecretKeyMultibase { get; set; } = "z42tqZ5smVag3DtDhjY9YfVwTMyVHW6SCHJi2ZMrD23DGYS3";
        public static string VerificationMethodId { get; set; } = $"{ControllerId}#{PublicKeyMultibase}";

        public static string GetVerificationMethodDocument()
        {
            var vm = new MultikeyVerificationMethod()
            {
                Id = VerificationMethodId,
                Controller = ControllerId,
                PublicKeyMultibase = PublicKeyMultibase,
            };
            return JObject.FromObject(vm).ToString();
        }
    }
}
