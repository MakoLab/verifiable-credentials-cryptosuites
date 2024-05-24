using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Formats.Cbor;

namespace ECDsa_sd_2023_Functions
{
    public class BaseProof
    {
        public static readonly byte[] BaseProofHeader = [0xd9, 0x5d, 0x00];
        public required string BaseSignature { get; set; }
        public required string PublicKey { get; set; }
        public required string HMACKey { get; set; }
        public required List<string> Signatures { get; set; }
        public required List<string> MandatoryPointers { get; set; }


        /// <summary>
        /// Serializes the base proof value.
        /// </summary>
        /// <returns>A single base proof string value.</returns>
        public string Serialize()
        {
            var endcoded = CborEncode();
            var array = new byte[BaseProofHeader.Length + endcoded.Length];
            Buffer.BlockCopy(BaseProofHeader, 0, array, 0, BaseProofHeader.Length);
            Buffer.BlockCopy(endcoded, 0, array, BaseProofHeader.Length, endcoded.Length);
            var multibase = ToBase64UrlNoPadding(array);
            return $"{Constants.MultibaseBase64Header}{multibase}";
        }

        public static BaseProof Deserialize(string baseProof)
        {
            if (baseProof[0] != Constants.MultibaseBase64Header)
            {
                throw new ArgumentException("Invalid multibase header");
            }
            var multibase = baseProof[1..];
            var array = FromBase64Url(multibase);
            if (!array.Take(BaseProofHeader.Length).SequenceEqual(BaseProofHeader))
            {
                throw new ArgumentException("Invalid base proof header");
            }
            return CborDecode(array[BaseProofHeader.Length..]);
        }

        /// <summary>
        /// Serializes the data that is to be signed by the private key associated with the base proof verification
        /// method.
        /// </summary>
        /// <param name="proofHash">Proof options hash.</param>
        /// <param name="publicKey">Proof-scoped multikey-encoded public key.</param>
        /// <param name="mandatoryHash">Mandatory hash.</param>
        /// <returns>A single sign data value, represented as series of bytes.</returns>
        public static string SerializeSignData(string proofHash, string publicKey, string mandatoryHash)
        {
            return String.Join("", proofHash, publicKey, mandatoryHash);
        }

        private byte[] CborEncode()
        {
            var cbor = new CborWriter();
            cbor.WriteStartArray(5);
            cbor.WriteByteString(Encoding.UTF8.GetBytes(BaseSignature));
            cbor.WriteByteString(Encoding.UTF8.GetBytes(PublicKey));
            cbor.WriteByteString(Encoding.UTF8.GetBytes(HMACKey));
            cbor.WriteStartArray(Signatures.Count);
            foreach (var signature in Signatures)
            {
                cbor.WriteByteString(Encoding.UTF8.GetBytes(signature));
            }
            cbor.WriteEndArray();
            cbor.WriteStartArray(MandatoryPointers.Count);
            foreach (var pointer in MandatoryPointers)
            {
                cbor.WriteTextString(pointer);
            }
            cbor.WriteEndArray();
            cbor.WriteEndArray();
            return cbor.Encode();
        }

        private static BaseProof CborDecode(byte[] data)
        {
            var cbor = new CborReader(data);
            cbor.ReadStartArray();
            var baseSignature = Encoding.UTF8.GetString(cbor.ReadByteString());
            var publicKey = Encoding.UTF8.GetString(cbor.ReadByteString());
            var hMACKey = Encoding.UTF8.GetString(cbor.ReadByteString());
            cbor.ReadStartArray();
            var signatures = new List<string>();
            while (cbor.PeekState() != CborReaderState.EndArray)
            {
                signatures.Add(Encoding.UTF8.GetString(cbor.ReadByteString()));
            }
            cbor.ReadEndArray();
            cbor.ReadStartArray();
            var mandatoryPointers = new List<string>();
            while (cbor.PeekState() != CborReaderState.EndArray)
            {
                mandatoryPointers.Add(cbor.ReadTextString());
            }
            cbor.ReadEndArray();
            cbor.ReadEndArray();
            return new BaseProof()
            {
                BaseSignature = baseSignature,
                PublicKey = publicKey,
                HMACKey = hMACKey,
                Signatures = signatures,
                MandatoryPointers = mandatoryPointers
            };
        }

        private static string ToBase64UrlNoPadding(byte[] data)
        {
            return Convert.ToBase64String(data).Replace('+', '-').Replace('/', '_').Remove('=');
        }

        private static byte[] FromBase64Url(string data)
        {
            var padding = data.Length % 4;
            if (padding > 0)
            {
                data += new string('=', 4 - padding);
            }
            return Convert.FromBase64String(data.Replace('-', '+').Replace('_', '/'));
        }
    }
}
