using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Formats.Cbor;
using Cryptosuite.Core.Util;
using Cryptosuite.Core;

namespace ECDsa_sd_2023_Functions
{
    public class BaseProof
    {
        public static readonly byte[] BaseProofHeader = [0xd9, 0x5d, 0x00];
        public required byte[] BaseSignature { get; set; }
        public required byte[] PublicKey { get; set; }
        public required byte[] HMACKey { get; set; }
        public required IList<byte[]> Signatures { get; set; }
        public required IList<string> MandatoryPointers { get; set; }


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
            var multibase = BaseConvert.ToBase64UrlNoPadding(array);
            return $"{Constants.MultibaseBase64Header}{multibase}";
        }

        /// <summary>
        /// Parses the components of an ecdsa-sd-2023 selective disclosure base proof value.
        /// </summary>
        /// <param name="baseProof">Cbor encoded, base64 string proof value.</param>
        /// <returns>Base proof, containing five elements, using the names baseSignature, publicKey, hmacKey,
        /// signatures, and mandatoryPointers.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static BaseProof Deserialize(string baseProof)
        {
            if (baseProof[0] != Constants.MultibaseBase64Header)
            {
                throw new ArgumentException("Invalid multibase header");
            }
            var multibase = baseProof[1..];
            var array = BaseConvert.FromBase64UrlNoPadding(multibase);
            if (!array.Take(BaseProofHeader.Length).SequenceEqual(BaseProofHeader))
            {
                throw new ArgumentException("Invalid base proof header");
            }
            return CborDecode(array[BaseProofHeader.Length..]);
        }

        /// <summary>
        /// Attempts to parse the components of an ecdsa-sd-2023 selective disclosure base proof value.
        /// </summary>
        /// <param name="baseProof"></param>
        /// <param name="proof"></param>
        /// <returns><c>True</c> if <paramref name="baseProof"/> was successfully parsed; otherwise <c>false</c>.</returns>
        public static bool TryDeserialize(string baseProof, out BaseProof? proof)
        {
            try
            {
                proof = Deserialize(baseProof);
                return true;
            }
            catch
            {
                proof = null;
                return false;
            }
        }

        /// <summary>
        /// Serializes the data that is to be signed by the private key associated with the base proof verification
        /// method.
        /// </summary>
        /// <param name="proofHash">Proof options hash.</param>
        /// <param name="publicKey">Proof-scoped multikey-encoded public key.</param>
        /// <param name="mandatoryHash">Mandatory hash.</param>
        /// <returns>A single sign data value, represented as series of bytes.</returns>
        public static byte[] SerializeSignData(byte[] proofHash, byte[] publicKey, byte[] mandatoryHash)
        {
            return [..proofHash, ..publicKey, ..mandatoryHash];
        }

        private byte[] CborEncode()
        {
            var cbor = new CborWriter();
            cbor.WriteStartArray(5);
            cbor.WriteByteString(BaseSignature);
            cbor.WriteByteString(PublicKey);
            cbor.WriteByteString(HMACKey);
            cbor.WriteStartArray(Signatures.Count);
            foreach (var signature in Signatures)
            {
                cbor.WriteByteString(signature);
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
            var baseSignature = cbor.ReadByteString();
            var publicKey = cbor.ReadByteString();
            var hMACKey = cbor.ReadByteString();
            cbor.ReadStartArray();
            var signatures = new List<byte[]>();
            while (cbor.PeekState() != CborReaderState.EndArray)
            {
                signatures.Add(cbor.ReadByteString());
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
    }
}
