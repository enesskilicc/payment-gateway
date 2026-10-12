using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PSP.APPLICATION.Merchants;
using PSP.APPLICATION.Payments;

namespace PSP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MerchantsController : ControllerBase
    {
        private readonly MerchantService _merchantService;

        public MerchantsController(MerchantService merchantService)
        {
            _merchantService = merchantService;
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateMerchantRequest request, CancellationToken cancellationToken)
        {
            var response = await _merchantService.CreateAsync(request, cancellationToken);

            return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var response = await _merchantService.GetByIdAsync(id, cancellationToken);

            if (response is null)
                return NotFound();

            return Ok(response);
        }
    }
}
