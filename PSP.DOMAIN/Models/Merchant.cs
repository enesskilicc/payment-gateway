using PSP.DOMAIN.Enums;

namespace PSP.DOMAIN.Models
{
    public class Merchant
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public MerchantStatus Status { get; private set; }
        public DateTimeOffset CreatedAt { get; private set; }


        public Merchant(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Merchant ismi geçersiz!", nameof(name));

            if(name.Length > 100)
                throw new ArgumentException("Merchant ismi çok uzun!", nameof(name));

            Id = Guid.NewGuid();
            Name = name;
            Status = MerchantStatus.Active;
            CreatedAt = DateTimeOffset.UtcNow;
        }

        public void Suspend()
        {
            if(Status != MerchantStatus.Active)
                throw new InvalidOperationException("Aktif olmayan merchant askıya alınamaz!");

            Status = MerchantStatus.Suspended;
        }

        public void Activate()
        {
            if (Status != MerchantStatus.Suspended)
                throw new InvalidOperationException("Askıya alınmamış merchant aktif edilemez!");

            Status = MerchantStatus.Active;
        }

        public void Delete()
        {
            if (Status == MerchantStatus.Deleted)
                throw new InvalidOperationException("Bu merchant zaten silinmiş!");

            Status = MerchantStatus.Deleted;
        }
    }
}
