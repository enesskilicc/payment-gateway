using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace PSP.DOMAIN.Models
{
    public class ApiKey
    {
        public Guid Id { get; private set; }
        public Guid MerchantId { get; private set; }
        public string KeyHash { get; private set; }
        public string Prefix { get; private set; }
        public DateTimeOffset CreatedAt { get; private set; }

        private ApiKey(Guid merchantId, string keyHash, string prefix)
        {
            Id = Guid.NewGuid();
            MerchantId = merchantId;
            KeyHash = keyHash;
            Prefix = prefix;
            CreatedAt = DateTimeOffset.UtcNow;
        }


        public static (ApiKey apiKey, string plainKey) Generate(Guid merchantId)
        {
            byte[] randomBytes = RandomNumberGenerator.GetBytes(32);
            string plainKey = "sk_test_" + Convert.ToBase64String(randomBytes);
            byte[] hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(plainKey));
            string keyHash = Convert.ToHexString(hashBytes);
            string prefix = plainKey.Substring(0, 12);
            var apiKey = new ApiKey(merchantId, keyHash, prefix);
            return (apiKey, plainKey);
        }

        public bool Verify(string candidateKey)
        {
            if (string.IsNullOrEmpty(candidateKey))
                return false;

            byte[] hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(candidateKey));
            string keyHash = Convert.ToHexString(hashBytes);

            return CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(keyHash), Encoding.UTF8.GetBytes(KeyHash));
        }

    }
}
