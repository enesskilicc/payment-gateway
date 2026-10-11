using PSP.DOMAIN.Models;

namespace PSP.APPLICATION.Payments
{
    public sealed record PaymentResponse(Guid Id,
    long Amount,
    long CapturedAmount,
    long RefundedAmount,
    string Currency,
    string Status,
    DateTimeOffset CreatedAt)
    {
        public static PaymentResponse FromDomain(Payment payment)
        {
            return new PaymentResponse(
                payment.Id,
                payment.Amount.Amount,
                payment.CapturedAmount.Amount,
                payment.RefundedAmount.Amount,
                payment.Amount.Currency,
                payment.Status.ToString(),
                payment.CreatedAt
                );
        }
    }
}
