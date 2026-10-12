using Microsoft.EntityFrameworkCore;
using PSP.APPLICATION.Abstractions;
using PSP.DOMAIN.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PSP.INFRASTRUCTURE.Persistence.Repositories
{
    public sealed class ApiKeyRepository : IApiKeyRepository
    {
        private readonly PspDbContext _context;

        public ApiKeyRepository(PspDbContext context)
        {
            _context = context;
        }

        public Task AddAsync(ApiKey apiKey, CancellationToken cancellationToken)
        {
            _context.ApiKeys.Add(apiKey);
            return Task.CompletedTask;
        }

        public Task<ApiKey?> GetByHashAsync(string keyHash, CancellationToken cancellationToken)
        {
            return _context.ApiKeys.FirstOrDefaultAsync(x => x.KeyHash == keyHash, cancellationToken);
        }
    }
}
