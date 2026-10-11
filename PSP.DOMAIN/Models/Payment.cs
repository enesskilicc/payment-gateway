using PSP.DOMAIN.Enums;
using PSP.DOMAIN.Exceptions;
namespace PSP.DOMAIN.Models
{
    public class Payment
    {
        public Guid Id { get; private set; }
        public Guid MerchantId { get; private set; }
        public Money Amount { get; private set; }
        public PaymentStatus Status { get; private set; }
        public DateTimeOffset CreatedAt { get; private set; }
        public Money CapturedAmount { get; private set; }
        public Money RefundedAmount { get; private set; } = default!;

        private Payment() { }

        public Payment(Guid merchantId, Money amount)
        {
            if (amount is null)
                throw new ArgumentNullException(nameof(amount), "Tutar bilgisi eksik!");
            if (amount.Amount <= 0)
                throw new DomainValidationException(DomainErrorCodes.InvalidAmount,"Tutar bilgisi hatalı!");

            Id = Guid.NewGuid();
            MerchantId = merchantId;
            Amount = amount;
            Status = PaymentStatus.Pending;
            CreatedAt = DateTimeOffset.UtcNow;
            CapturedAmount = new Money(0, amount.Currency);
            RefundedAmount = new Money(0, amount.Currency);
        }

        public void Authorize()
        {
            if (Status != PaymentStatus.Pending)
                throw new DomainConflictException(DomainErrorCodes.PaymentNotAuthorizable,"İşlem otorizasyon için uygun değil!");

            Status = PaymentStatus.Authorized;
        }

        public void Capture(Money amount)
        {
            if (amount is null)
                throw new ArgumentNullException(nameof(amount), "Tutar bilgisi eksik!");

            if (amount.Amount <= 0)
                throw new DomainValidationException(DomainErrorCodes.InvalidAmount,"Geçersiz tutar!");

            if (Status != PaymentStatus.Authorized)
                throw new DomainConflictException(DomainErrorCodes.PaymentNotCapturable,"Yalnızca otorize edilmiş ödeme tahsil edilebilir!");

            if (amount.IsGreaterThan(Amount))
                throw new DomainValidationException(DomainErrorCodes.AmountExceedsAuthorized, "Tahsil tutarı bloke edilen tutardan büyük olamaz!");

            CapturedAmount = amount;
            Status = PaymentStatus.Captured;
        }

        public void Void()
        {
            if (Status != PaymentStatus.Authorized)
                throw new DomainConflictException(DomainErrorCodes.PaymentNotVoidable, "Bloke kaldırma işlemi için otorizasyon yapılmamış!");

            Status = PaymentStatus.Voided;
        }

        public void Refund(Money amount)
        {
            if (amount is null)
                throw new ArgumentNullException(nameof(amount), "Tutar bilgisi eksik!");

            if (amount.Amount <= 0)
                throw new DomainValidationException(DomainErrorCodes.InvalidAmount, "Geçersiz tutar!");

            if (Status != PaymentStatus.Captured && Status != PaymentStatus.PartiallyRefunded)
                throw new DomainConflictException(DomainErrorCodes.PaymentNotRefundable, "Tahsil edilmemiş işlem iade edilemez!");

            var refundable = CapturedAmount.Subtract(RefundedAmount);

            if (amount.IsGreaterThan(refundable))
                throw new DomainValidationException(DomainErrorCodes.AmountExceedsRefundable, "İade edilecek tutar kalan iade tutarından büyük!");

            RefundedAmount = RefundedAmount.Add(amount);

            if (RefundedAmount == CapturedAmount)
                Status = PaymentStatus.Refunded;
            else
                Status = PaymentStatus.PartiallyRefunded;

        }

        public void Fail()
        {
            if (Status != PaymentStatus.Pending)
                throw new DomainConflictException(DomainErrorCodes.PaymentAlreadyProcessed, "İşlem zaten başlamış!");

            Status = PaymentStatus.Failed;
        }
    }
}
