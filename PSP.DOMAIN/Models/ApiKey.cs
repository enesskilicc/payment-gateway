using PSP.DOMAIN.Exceptions;
using System.Buffers.Text;
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
        public DateTimeOffset? RevokedAt { get; private set; }

        private ApiKey(Guid merchantId, string keyHash, string prefix)
        {
            Id = Guid.NewGuid();
            MerchantId = merchantId;
            KeyHash = keyHash;
            Prefix = prefix;
            CreatedAt = DateTimeOffset.UtcNow;
        }

        public void Revoke()
        {
            if (RevokedAt is not null)
                throw new DomainConflictException(DomainErrorCodes.ApiKeyAlreadyRevoked,"Key zaten iptal edilmiş!");

            RevokedAt = DateTimeOffset.UtcNow;
        }


        public static (ApiKey apiKey, string plainKey) Generate(Guid merchantId)
        {
            byte[] randomBytes = RandomNumberGenerator.GetBytes(32);
            string plainKey = "sk_test_" + Base64Url.EncodeToString(randomBytes);
            var hash = ComputeHash(plainKey);
            string prefix = plainKey.Substring(0, 12);
            var apiKey = new ApiKey(merchantId, hash, prefix);
            return (apiKey, plainKey);
        }

        public bool Verify(string candidateKey)
        {
            if (string.IsNullOrEmpty(candidateKey) || RevokedAt is not null)
                return false;

            var hash = ComputeHash(candidateKey);

            return CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(hash), Encoding.UTF8.GetBytes(KeyHash));
        }

        public static string ComputeHash(string plainKey)
        {
            byte[] hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(plainKey));
            string keyHash = Convert.ToHexString(hashBytes);
            return keyHash;
        }

    }
}
