using Cryptosuite.Core;
using Cryptosuite.Core.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDsa_sd_2023_Functions.Util
{
    public static class LabelMapCompressor
    {
        public static Dictionary<string, byte[]> CompressLabelMap(Dictionary<string, string> labelMap)
        {
            var compressedLabelMap = new Dictionary<string, byte[]>();
            foreach (var (key, value) in labelMap)
            {
                compressedLabelMap.Add(key.Replace("c14n", ""), BaseConvert.FromBase64UrlNoPadding(value[1..]));
            }
            return compressedLabelMap;
        }

        public static Dictionary<string, string> DecompressLabelMap(Dictionary<string, byte[]> compressedLabelMap)
        {
            var labelMap = new Dictionary<string, string>();
            foreach (var (key, value) in compressedLabelMap)
            {
                labelMap.Add($"c14n{key}", $"{Constants.MultibaseBase64Header}{BaseConvert.ToBase64UrlNoPadding(value)}");
            }
            return labelMap;
        }
    }
}
