using ECDsa_Multikey;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDsa_sd_2023_Cryptosuite.Tests
{
    internal class MockData
    {
        public static string Controller { get; set; } = "http://localhost:40443/issuers";
        public static string PublicKeyMultibase { get; set; } = "zDnaekGZTbQBerwcehBSXLqAg6s55hVEBms1zFy89VHXtJSa9";
        public static string SecretKeyMultibase { get; set; } = "z42tqZ5smVag3DtDhjY9YfVwTMyVHW6SCHJi2ZMrD23DGYS3";
        public static string Id { get; set; } = $"{Controller}#{PublicKeyMultibase}";

        public static string GetVerificationMethodDocument()
        {
            var vm = new MultikeyModel()
            {
                Id = Id,
                Type = "Multikey",
                Controller = Controller,
                PublicKeyMultibase = PublicKeyMultibase,
                AssertionMethod = new MultikeyModel[] { new() { Id = Id, Type = "Multikey", Controller = Controller, PublicKeyMultibase = PublicKeyMultibase } }
            };
            return JObject.FromObject(vm).ToString();
        }
    }
}
