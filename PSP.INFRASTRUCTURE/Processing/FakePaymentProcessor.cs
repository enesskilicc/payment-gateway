using PSP.APPLICATION.Abstractions;
using PSP.APPLICATION.PaymentProcessor;
using PSP.DOMAIN.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PSP.INFRASTRUCTURE.Processing
{
    public sealed class FakePaymentProcessor : IPaymentProcessor
    {
        public Task<AuthorizationResult> AuthorizeAsync(Payment payment, CancellationToken cancellationToken)
        {
            return Task.FromResult(new AuthorizationResult(true, null));
        }
    }
}
