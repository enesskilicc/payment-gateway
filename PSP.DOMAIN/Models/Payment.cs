using PSP.DOMAIN.Enums;
using System;
using System.Collections.Generic;
using System.Text;

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
        public Money RefundedAmount { get; private set; }


        public Payment(Guid merchantId, Money amount)
        {
            if (amount is null)
                throw new ArgumentNullException(nameof(amount), "Tutar bilgisi eksik!");
            if (amount.Amount <= 0)
                throw new ArgumentException("Tutar bilgisi hatalı!", nameof(amount));

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
                throw new InvalidOperationException("İşlem otorizasyon için uygun değil!");

            Status = PaymentStatus.Authorized;
        }

        public void Capture(Money amount)
        {
            if (amount is null)
                throw new ArgumentNullException(nameof(amount), "Tutar bilgisi eksik!");

            if (amount.Amount <= 0)
                throw new ArgumentException("Geçersiz tutar!", nameof(amount));

            if (Status != PaymentStatus.Authorized)
                throw new InvalidOperationException("Yalnızca otorize edilmiş ödeme tahsil edilebilir!");

            if (amount.IsGreaterThan(Amount))
                throw new ArgumentException("Tahsil tutarı bloke edilen tutardan büyük olamaz!", nameof(amount));

            CapturedAmount = amount;
            Status = PaymentStatus.Captured;
        }

        public void Void()
        {
            if (Status != PaymentStatus.Authorized)
                throw new InvalidOperationException("Bloke kaldırma işlemi için otorizasyon yapılmamış!");

            Status = PaymentStatus.Voided;
        }

        public void Refund(Money amount)
        {
            if (amount is null)
                throw new ArgumentNullException(nameof(amount), "Tutar bilgisi eksik!");

            if (amount.Amount <= 0)
                throw new ArgumentException("Geçersiz tutar!", nameof(amount));

            if (Status != PaymentStatus.Captured && Status != PaymentStatus.PartiallyRefunded)
                throw new InvalidOperationException("Tahsil edilmemiş işlem iade edilemez!");

            var refundable = CapturedAmount.Subtract(RefundedAmount);

            if (amount.IsGreaterThan(refundable))
                throw new ArgumentException("İade edilecek tutar kalan iade tutarından büyük!", nameof(amount));

            RefundedAmount = RefundedAmount.Add(amount);

            if (RefundedAmount == CapturedAmount)
                Status = PaymentStatus.Refunded;
            else
                Status = PaymentStatus.PartiallyRefunded;

        }

        public void Fail()
        {
            if (Status != PaymentStatus.Pending)
                throw new InvalidOperationException("İşlem zaten başlamış!");

            Status = PaymentStatus.Failed;
        }
    }
}
