using System.Security.Claims;

namespace PSP.API.Authentication
{
    public static class ClaimsPrincipalExtensions
    {
        public static Guid GetMerchantId(this ClaimsPrincipal user) =>
            Guid.Parse(user.FindFirstValue(ApiKeyAuthenticationHandler.MerchantIdClaim)!);
    }
}
