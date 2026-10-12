using PSP.APPLICATION.Abstractions;
using PSP.DOMAIN.Models;

namespace PSP.APPLICATION.Merchants
{
    public sealed class MerchantService
    {
        private readonly IMerchantRepository _merchantRepository;
        private readonly IUnitOfWork _unitOfWork;

        public MerchantService(IMerchantRepository merchantRepository, IUnitOfWork unitOfWork, IApiKeyRepository apiKeyRepository)
        {
            _merchantRepository = merchantRepository;
            _unitOfWork = unitOfWork;
        }


        public async Task<MerchantResponse> CreateAsync(CreateMerchantRequest createMerchantRequest, CancellationToken cancellationToken)
        {
            var merchant = new Merchant(createMerchantRequest.MerchantName);

            await _merchantRepository.AddAsync(merchant, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return MerchantResponse.FromDomain(merchant);

        }


        public async Task<MerchantResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var merchant = await _merchantRepository.GetByIdAsync(id, cancellationToken);
            if (merchant is null)
                return null;

            return MerchantResponse.FromDomain(merchant);
        }
    }
}
