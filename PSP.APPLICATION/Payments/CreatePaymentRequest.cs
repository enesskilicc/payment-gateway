namespace PSP.APPLICATION.Payments
{
    public sealed record CreatePaymentRequest(long Amount, string Currency,string MerchantOrderId);
}
