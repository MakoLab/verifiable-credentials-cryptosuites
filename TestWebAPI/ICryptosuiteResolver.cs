using Cryptosuite.Core.Interfaces;

namespace TestWebAPI
{
    public interface ICryptosuiteResolver
    {
        ICryptosuite? GetCryptosuite(string cryptoId);
        bool RegisterCryptosuiteType(string cryptoId, Type cryptosuiteType);
    }
}