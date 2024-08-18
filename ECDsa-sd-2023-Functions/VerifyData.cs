namespace ECDsa_sd_2023_Functions
{
    public class VerifyData
    {
        public required byte[] BaseSignature { get; set; }
        public required byte[] ProofHash { get; set; }
        public required byte[] PublicKey { get; set; }
        public required List<byte[]> Signatures { get; set; }
        public required List<string> NonMandatory { get; set; }
        public required byte[] MandatoryHash { get; set; }
    }
}
