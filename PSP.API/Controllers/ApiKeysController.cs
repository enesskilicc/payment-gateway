using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PSP.APPLICATION.ApiKeys;

namespace PSP.API.Controllers
{
    [Route("api/merchants/{merchantId:guid}/api-keys")]
    [ApiController]
    public class ApiKeysController : ControllerBase
    {
        private readonly ApiKeyService _apiKeyService;

        public ApiKeysController(ApiKeyService apiKeyService)
        {
            _apiKeyService = apiKeyService;
        }
        [HttpPost]
        public async Task<IActionResult> Create(Guid merchantId,CancellationToken cancellationToken)
        {
            var apiKey = await _apiKeyService.CreateAsync(merchantId,cancellationToken);

            if (apiKey is null)
                return NotFound();

            return StatusCode(201,apiKey);
        }
    }
}
