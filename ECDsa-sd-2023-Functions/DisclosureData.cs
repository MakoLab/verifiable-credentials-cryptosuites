
using Cryptosuite.Core;
using Cryptosuite.Core.Util;
using ECDsa_sd_2023_Functions.Util;
using Newtonsoft.Json.Linq;
using PeterO.Cbor;
using ECDsa_Multikey;

namespace ECDsa_sd_2023_Functions
{
    public class DisclosureData
    {
        public static readonly byte[] DisclosureProofHeader = [0xd9, 0x5d, 0x01];
        public required byte[] BaseSignature { get; set; }
        public required byte[] PublicKey { get; set; }
        public required List<byte[]> Signatures { get; set; }
        public required Dictionary<string, string> LabelMap { get; set; }
        public required List<int> MandatoryIndexes { get; set; }
        public JObject? RevealDocument { get; set; }


        /// <summary>
        /// Serializes a derived proof value.
        /// </summary>
        /// <returns>A single derived proof value, serialized as a multibase string.</returns>
        public string SerializeDerivedProofValue()
        {
            var compressedLabelMap = LabelMapCompressor.CompressLabelMap(LabelMap);
            var cbor = CBORObject.NewArray()
                .Add(BaseSignature)
                .Add(PublicKey)
                .Add(Signatures)
                .Add(compressedLabelMap)
                .Add(MandatoryIndexes);
            var encoded = cbor.EncodeToBytes();
            var array = new byte[DisclosureProofHeader.Length + encoded.Length];
            Buffer.BlockCopy(DisclosureProofHeader, 0, array, 0, DisclosureProofHeader.Length);
            Buffer.BlockCopy(encoded, 0, array, DisclosureProofHeader.Length, encoded.Length);
            return $"{Constants.MultibaseBase64Header}{BaseConvert.ToBase64UrlNoPadding(array)}";
        }

        /// <summary>
        /// Parses the components of the derived proof value.
        /// </summary>
        /// <param name="derivedProofValue">A derived proof value multibase string.</param>
        /// <returns>A derived proof value object.</returns>
        public static DisclosureData ParseDerivedProofValue(string derivedProofValue)
        {
            if (derivedProofValue[0] != Constants.MultibaseBase64Header)
            {
                throw new ArgumentException("Invalid multibase header");
            }
            var multibase = derivedProofValue[1..];
            var array = BaseConvert.FromBase64UrlNoPadding(multibase);
            if (!array.Take(DisclosureProofHeader.Length).SequenceEqual(DisclosureProofHeader))
            {
                throw new ArgumentException("Invalid disclosure proof header");
            }
            var cbor = CBORObject.DecodeFromBytes(array[DisclosureProofHeader.Length..]);
            var baseSignature = cbor[0].ToObject<byte[]>();
            var publicKey = cbor[1].ToObject<byte[]>();
            var signatures = cbor[2].ToObject<List<byte[]>>();
            var compressedLabelMap = cbor[3].ToObject<Dictionary<string, byte[]>>();
            var labelMap = LabelMapCompressor.DecompressLabelMap(compressedLabelMap);
            var mandatoryIndexes = cbor[4].ToObject<List<int>>();
            return new DisclosureData
            {
                BaseSignature = baseSignature,
                PublicKey = publicKey,
                Signatures = signatures,
                LabelMap = labelMap,
                MandatoryIndexes = mandatoryIndexes
            };
        }

        /// <summary>
        /// Attempts to parse the components of a derived proof value.
        /// </summary>
        /// <param name="derivedProofValue">A derived proof value multibase string.</param>
        /// <param name="proof">A derived proof value object.</param>
        /// <returns><c>True</c> if <paramref name="derivedProofValue"/> was successfully parsed; otherwise <c>false</c>.</returns>
        public static bool TryParseDerivedProofValue(string derivedProofValue, out DisclosureData? proof)
        {
            try
            {
                proof = ParseDerivedProofValue(derivedProofValue);
                return true;
            }
            catch
            {
                proof = null;
                return false;
            }
        }
    }
}