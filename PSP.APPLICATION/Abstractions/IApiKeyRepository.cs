using PSP.DOMAIN.Models;

namespace PSP.APPLICATION.Abstractions
{
    public interface IApiKeyRepository
    {
        Task AddAsync(ApiKey apiKey, CancellationToken cancellationToken);
        Task<ApiKey?> GetByHashAsync(string keyHash, CancellationToken cancellationToken);
        Task<ApiKey?> GetByIdAsync(Guid id, Guid merchantId, CancellationToken cancellationToken);
    }
}
