using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PSP.APPLICATION.Abstractions;
using PSP.APPLICATION.Payments;
using PSP.INFRASTRUCTURE.Persistence.Repositories;

namespace PSP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly PaymentService _paymentService;

        public PaymentsController(PaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Create([FromHeader(Name = "X-Merchant-Id")] Guid merchantId,CreatePaymentRequest request,CancellationToken cancellationToken)
        {
            var response = await _paymentService.CreateAsync(merchantId,request,cancellationToken);

            return CreatedAtAction(nameof(GetById), new { Id = response.Id }, response);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, [FromHeader(Name = "x-Merchant-Id")] Guid merchantId,CancellationToken cancellationToken)
        {
            var response = await _paymentService.GetByIdAsync(id,merchantId,cancellationToken);

            if(response is null)
                return NotFound();

            return Ok(response);
        }
    }
}
