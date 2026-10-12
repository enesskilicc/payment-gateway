using Microsoft.Extensions.Logging;
using PSP.APPLICATION.Abstractions;
using PSP.DOMAIN.Enums;
using PSP.DOMAIN.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PSP.APPLICATION.Authentication
{
    public sealed class ApiKeyAuthenticator
    {
        private readonly IApiKeyRepository _apiKeyRepository;
        private readonly IMerchantRepository _merchantRepository;
        private readonly ILogger<ApiKeyAuthenticator> _logger;

        public ApiKeyAuthenticator(IApiKeyRepository apiKeyRepository, IMerchantRepository merchantRepository, ILogger<ApiKeyAuthenticator> logger)
        {
            _apiKeyRepository = apiKeyRepository;
            _merchantRepository = merchantRepository;
            _logger = logger;
        }


        public async Task<Guid?> AuthenticateAsync(string? plainKey, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(plainKey))
                return null;

            var keyHash = ApiKey.ComputeHash(plainKey);

            var apiKey = await _apiKeyRepository.GetByHashAsync(keyHash, cancellationToken);

            if (apiKey is null)
                return null;

            if (apiKey.RevokedAt is not null)
            {
                _logger.LogWarning($"İptal edilmiş API key kullanıldı! Prefix: {apiKey.Prefix}, MerchantId: {apiKey.MerchantId}");
                return null;
            }

            var merchant = await _merchantRepository.GetByIdAsync(apiKey.MerchantId, cancellationToken);

            if (merchant is null)
                return null;

            if (merchant.Status == MerchantStatus.Deleted)
                return null;

            return merchant.Id;
        }
    }
}
