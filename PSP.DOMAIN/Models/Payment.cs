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


        public Payment(Guid merchantId, Money amount)
        {
            if(amount is null)
                throw new ArgumentNullException(nameof(amount),"Tutar bilgisi eksik!");
            if (amount.Amount <= 0)
                throw new ArgumentException("Tutar bilgisi hatalı!", nameof(amount));

            Id = Guid.NewGuid();
            MerchantId = merchantId;
            Amount = amount;
            Status = PaymentStatus.Pending;
            CreatedAt = DateTimeOffset.UtcNow;
        }

        public void Authorize()
        {
            if (Status != PaymentStatus.Pending)
                throw new InvalidOperationException("İşlem otorizasyon için uygun değil!");

            Status = PaymentStatus.Authorized;
        }

        public void Capture()
        {
            if (Status != PaymentStatus.Authorized)
                throw new InvalidOperationException("Yalnızca otorize edilmiş ödeme tahsil edilebilir!");

            Status = PaymentStatus.Captured;
        }

        public void Void()
        {
            if (Status != PaymentStatus.Authorized)
                throw new InvalidOperationException("Bloke kaldırma işlemi için otorizasyon yapılmamış!");

            Status = PaymentStatus.Voided;
        }

        public void Refund()
        {
            if (Status != PaymentStatus.Captured)
                throw new InvalidOperationException("Tahsil edilmemiş işlem iade edilemez!");

            Status = PaymentStatus.Refunded;
        }

        public void Fail()
        {
            if (Status != PaymentStatus.Pending)
                throw new InvalidOperationException("İşlem zaten başlamış!");

            Status = PaymentStatus.Failed;
        }
    }
}
