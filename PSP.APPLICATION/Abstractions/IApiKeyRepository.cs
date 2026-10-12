using PSP.DOMAIN.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PSP.APPLICATION.Abstractions
{
    public interface IApiKeyRepository
    {
        Task AddAsync(ApiKey apiKey,CancellationToken cancellationToken);
        Task<ApiKey?> GetByHashAsync(string keyHash, CancellationToken cancellationToken);
    }
}
