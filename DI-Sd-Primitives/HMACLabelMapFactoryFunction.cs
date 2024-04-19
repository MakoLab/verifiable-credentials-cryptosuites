using DI_Sd_Primitives.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DI_Sd_Primitives
{
    public class HMACLabelMapFactoryFunction(HMAC hMAC) : ILabelMapFactoryFunction
    {
        private readonly HMAC _hMAC = hMAC;

        public Dictionary<string, string> CreateLabelMap(Dictionary<string, string> canonicalIdMap)
        {
            var bnodeIdMap = new Dictionary<string, string>();
            foreach (var entry in canonicalIdMap)
            {
                var value = Encoding.UTF8.GetBytes(entry.Value);
                var hash = _hMAC.ComputeHash(value);
                var label = ToBase64UrlNoPadding(hash);
                bnodeIdMap.Add(entry.Key, label);
            }
            return bnodeIdMap;
        }

        private static string ToBase64UrlNoPadding(byte[] input)
        {
            return Convert.ToBase64String(input).Replace('+', '-').Replace('/', '_').TrimEnd('=');
        }
    }
}
