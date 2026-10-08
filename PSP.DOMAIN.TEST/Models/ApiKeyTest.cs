using PSP.DOMAIN.Models;

namespace PSP.DOMAIN.TEST.Models
{
    public class ApiKeyTest
    {
        [Fact]
        public void Generate_PlainKey_StartsWithPrefix()
        {
            var (apiKey, plainKey) = ApiKey.Generate(Guid.NewGuid());

            Assert.StartsWith("sk_test_", plainKey);
        }

        [Fact]
        public void Generate_TwoCalls_ProduceDifferentKeys()
        {
            var merchantId = Guid.NewGuid();
            var (_, plainKey) = ApiKey.Generate(merchantId);
            var (_, plainKey2) = ApiKey.Generate(merchantId);

            Assert.NotEqual(plainKey, plainKey2);
        }

        [Fact]
        public void Generate_KeyHash_DoesNotContainPlainKey()
        {
            var (apiKey, plainKey) = ApiKey.Generate(Guid.NewGuid());

            Assert.NotEqual(plainKey, apiKey.KeyHash);

            Assert.DoesNotContain(plainKey, apiKey.KeyHash);
        }

        [Fact]
        public void Generate_Prefix_IsFirst12CharsOfKey()
        {
            var (apiKey,plainKey) = ApiKey.Generate(Guid.NewGuid());

            Assert.Equal(plainKey.Substring(0,12),apiKey.Prefix);
        }

        [Fact]
        public void Verify_CorrectKey_ReturnsTrue()
        {
            var (apiKey, plainKey) = ApiKey.Generate(Guid.NewGuid());

            var result = apiKey.Verify(plainKey);

            Assert.True(result);
        }

        [Fact]
        public void Verify_WrongKey_ReturnsFalse()
        {
            var (apiKey, _) = ApiKey.Generate(Guid.NewGuid());
            var (_, otherKey) = ApiKey.Generate(Guid.NewGuid());

            var result = apiKey.Verify(otherKey);

            Assert.False(result);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Verify_EmptyKey_ReturnsFalse(string? candidateKey)
        {
            var (apiKey, _) = ApiKey.Generate(Guid.NewGuid());

            var result = apiKey.Verify(candidateKey);

            Assert.False(result);
        }
    }
}
