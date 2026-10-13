using System;
using System.Collections.Generic;
using System.Text;

namespace PSP.APPLICATION.PaymentProcessor
{
    public sealed record AuthorizationResult(bool Approved, string?DeclineCode);
}
