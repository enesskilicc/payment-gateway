using PSP.DOMAIN.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PSP.APPLICATION.ApiKeys
{
    public sealed record CreateApiKeyResponse(Guid Id, string ApiKey, string Prefix, DateTimeOffset CreatedAt);

    public sealed record ApiKeyResponse(Guid Id, string Prefix, DateTimeOffset CreatedAt, DateTimeOffset? RevokedAt)
    {
        public static ApiKeyResponse FromDomain(ApiKey apiKey) => new(apiKey.Id, apiKey.Prefix, apiKey.CreatedAt, apiKey.RevokedAt);
    }
}
