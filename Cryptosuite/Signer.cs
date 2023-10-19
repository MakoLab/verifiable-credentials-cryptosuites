namespace Cryptosuite
{
    public class Signer
    {
        public string? Id { get; set; }
        public string? Algorithm { get; set; }
        public string? VerificationMethod { get; internal set; }

        public byte[] Sign(object data)
        {
            throw new System.NotImplementedException();
        }
    }
}