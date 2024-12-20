using Cryptosuite.Core.Util;
using ECDsa_Multikey;

namespace ECDsa_sd_2023_Functions.Util
{
    public static class LabelMapCompressor
    {
        public static Dictionary<int, byte[]> CompressLabelMap(Dictionary<string, string> labelMap)
        {
            var compressedLabelMap = new Dictionary<int, byte[]>();
            foreach (var (key, value) in labelMap)
            {
                var counter = key.Replace("c14n", "");
                var success = int.TryParse(counter, out var index);
                if (!success)
                {
                    throw new ArgumentException("Invalid label map key");
                }
                compressedLabelMap.Add(index, BaseConvert.FromBase64UrlNoPadding(value[1..]));
            }
            return compressedLabelMap;
        }

        public static Dictionary<string, string> DecompressLabelMap(Dictionary<int, byte[]> compressedLabelMap)
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
