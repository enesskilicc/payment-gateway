using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PSP.API.Authentication;
using PSP.APPLICATION.Payments;

namespace PSP.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentsController : ControllerBase
    {
        private readonly PaymentService _paymentService;

        public PaymentsController(PaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreatePaymentRequest request,CancellationToken cancellationToken)
        {
            var merchantId = User.GetMerchantId();
            var response = await _paymentService.CreateAsync(merchantId,request,cancellationToken);

            return CreatedAtAction(nameof(GetById), new { Id = response.Id }, response);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var merchantId = User.GetMerchantId();
            var response = await _paymentService.GetByIdAsync(id,merchantId,cancellationToken);

            if(response is null)
                return NotFound();

            return Ok(response);
        }

        [HttpPost("{id:guid}/confirm")]
        public async Task<IActionResult> Confirm(Guid id,CancellationToken cancellationToken)
        {
            var merchantId = User.GetMerchantId();
            var response = await _paymentService.ConfirmAsync(id,merchantId,cancellationToken);

            if (response is null)
                return NotFound();

            return Ok(response);
        }
    }
}
