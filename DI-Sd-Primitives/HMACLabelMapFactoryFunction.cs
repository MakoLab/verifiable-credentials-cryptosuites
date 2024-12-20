using Cryptosuite.Core.Util;
using DI_Sd_Primitives.Interfaces;
using Org.BouncyCastle.Crypto.Macs;
using System.Text;

namespace DI_Sd_Primitives
{
    public class HMACLabelMapFactoryFunction(HMac hMAC) : ILabelMapFactoryFunction
    {
        private readonly HMac _hMAC = hMAC;

        public IDictionary<string, string> CreateLabelMap(IDictionary<string, string> canonicalIdMap)
        {
            var bnodeIdMap = new Dictionary<string, string>();
            foreach (var entry in canonicalIdMap)
            {
                var internalId = entry.Value;
                if (entry.Value.StartsWith("_:"))
                {
                    internalId = internalId[2..];
                }
                var value = Encoding.UTF8.GetBytes(internalId);
                _hMAC.BlockUpdate(value, 0, value.Length);
                var hash = new byte[_hMAC.GetMacSize()];
                _hMAC.DoFinal(hash, 0);
                _hMAC.Reset();
                var label = BaseConvert.ToBase64UrlNoPadding(hash);
                bnodeIdMap.Add(entry.Key, label);
            }
            return bnodeIdMap;
        }
    }
}
