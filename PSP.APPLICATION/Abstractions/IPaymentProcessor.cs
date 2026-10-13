using PSP.APPLICATION.PaymentProcessor;
using PSP.DOMAIN.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PSP.APPLICATION.Abstractions
{
    public interface IPaymentProcessor
    {
        Task<AuthorizationResult> AuthorizeAsync(Payment payment,CancellationToken cancellationToken );
    }
}
