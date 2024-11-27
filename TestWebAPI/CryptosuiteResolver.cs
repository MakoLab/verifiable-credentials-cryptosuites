using Cryptosuite.Core.Interfaces;
using System;
using System.Collections.Concurrent;

namespace TestWebAPI
{
    public class CryptosuiteResolver : ICryptosuiteResolver
    {
        private readonly ILogger<CryptosuiteResolver> _logger;
        private readonly ConcurrentDictionary<string, Type> _cryptosuiteTypes = new();

        public CryptosuiteResolver(ILogger<CryptosuiteResolver> logger)
        {
            _logger = logger;
        }

        public bool RegisterCryptosuiteType(string cryptoId, Type cryptosuiteType)
        {
            if (String.IsNullOrWhiteSpace(cryptoId))
            {
                _logger.LogWarning("Cryptosuite {cryptoId} is invalid", cryptoId);
                return false;
            }
            if (!typeof(ICryptosuite).IsAssignableFrom(cryptosuiteType))
            {
                _logger.LogWarning("Cryptosuite type {cryptosuiteType} does not implement ICryptosuite", cryptosuiteType);
                return false;
            }
            if (!_cryptosuiteTypes.TryAdd(cryptoId, cryptosuiteType))
            {
                _logger.LogWarning("Cryptosuite {cryptoId} already registered", cryptoId);
                return false;
            }
            return true;
        }

        public ICryptosuite? GetCryptosuite(string cryptoId)
        {
            if (_cryptosuiteTypes.TryGetValue(cryptoId, out var type))
            {
                return Activator.CreateInstance(type) as ICryptosuite;
            }
            _logger.LogWarning("Cryptosuite {cryptoId} not found", cryptoId);
            return null;
        }
    }
}
