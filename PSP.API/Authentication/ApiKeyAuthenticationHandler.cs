using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PSP.APPLICATION.Authentication;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace PSP.API.Authentication
{
    public sealed class ApiKeyAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public const string SchemeName = "ApiKey";
        public const string MerchantIdClaim = "merchant_id";
        private readonly ApiKeyAuthenticator _authenticator;

        public ApiKeyAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger, UrlEncoder encoder, ApiKeyAuthenticator authenticator)
        : base(options, logger, encoder)
        {
            _authenticator = authenticator;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            string? header = Request.Headers.Authorization;
            if (string.IsNullOrEmpty(header) || !header.StartsWith("Bearer ", StringComparison.Ordinal))
                return AuthenticateResult.NoResult();

            var plainKey = header["Bearer ".Length..].Trim();
            var merchantId = await _authenticator.AuthenticateAsync(plainKey, Context.RequestAborted);
            if (merchantId is null)
                return AuthenticateResult.Fail("Geçersiz API key.");

            var identity = new ClaimsIdentity(
                [new Claim(MerchantIdClaim, merchantId.Value.ToString())], SchemeName);
            var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), SchemeName);
            return AuthenticateResult.Success(ticket);
        }

        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Response.StatusCode = StatusCodes.Status401Unauthorized;
            Response.Headers.WWWAuthenticate = "Bearer";

            var problemDetailsService = Context.RequestServices.GetRequiredService<IProblemDetailsService>();
            await problemDetailsService.WriteAsync(new ProblemDetailsContext
            {
                HttpContext = Context,
                ProblemDetails = new ProblemDetails
                {
                    Status = StatusCodes.Status401Unauthorized,
                    Title = "Kimlik doğrulama başarısız!",
                    Detail = "Geçerli bir API key gönderilmedi.",
                    Extensions = { ["code"] = "invalid_api_key" }
                }
            });
        }
    }
}
