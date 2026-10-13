using PSP.APPLICATION.Abstractions;
using PSP.DOMAIN.Models;

namespace PSP.APPLICATION.ApiKeys
{
    public sealed class ApiKeyService
    {
        private readonly IApiKeyRepository _apiKeyRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMerchantRepository _merchantRepository;


        public ApiKeyService(IApiKeyRepository apiKeyRepository, IUnitOfWork unitOfWork, IMerchantRepository merchantRepository)
        {
            _apiKeyRepository = apiKeyRepository;
            _unitOfWork = unitOfWork;
            _merchantRepository = merchantRepository;
        }


        public async Task<CreateApiKeyResponse?> CreateAsync(Guid merchantId, CancellationToken cancellationToken)
        {
            var merchant = await _merchantRepository.GetByIdAsync(merchantId, cancellationToken);

            if (merchant is null)
                return null;

            merchant.EnsureActive();

            var (apiKey, plainKey) = ApiKey.Generate(merchant.Id);

            await _apiKeyRepository.AddAsync(apiKey, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new CreateApiKeyResponse(apiKey.Id, plainKey, apiKey.Prefix, apiKey.CreatedAt);
        }

        public async Task<bool> RevokeAsync(Guid keyId, Guid merchantId, CancellationToken cancellationToken)
        {
            var apiKey = await _apiKeyRepository.GetByIdAsync(keyId,merchantId,cancellationToken);

            if (apiKey is null)
                return false;

            apiKey.Revoke();
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
