using Cryptosuite.Core.ControllerDocuments;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDsa_Multikey
{
    public class MultikeyVerificationMethod : VerificationMethod
    {
        public string? PublicKeyMultibase { get; set; }
        public string? SecretKeyMultibase { get; set; }

        [SetsRequiredMembers]
        public MultikeyVerificationMethod(string id, string controller)
        {
            Type = "Multikey";
            Id = id;
            Controller = controller;
        }
    }
}
