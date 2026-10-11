using PSP.DOMAIN.Enums;
using PSP.DOMAIN.Exceptions;

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
                throw new DomainValidationException(DomainErrorCodes.InvalidMerchantName,"Merchant ismi geçersiz!");

            if(name.Length > 100)
                throw new DomainValidationException(DomainErrorCodes.InvalidMerchantName, "Merchant ismi çok uzun!");

            Id = Guid.NewGuid();
            Name = name;
            Status = MerchantStatus.Active;
            CreatedAt = DateTimeOffset.UtcNow;
        }

        public void Suspend()
        {
            if(Status != MerchantStatus.Active)
                throw new DomainConflictException(DomainErrorCodes.MerchantNotActive,"Aktif olmayan merchant askıya alınamaz!");

            Status = MerchantStatus.Suspended;
        }

        public void Activate()
        {
            if (Status != MerchantStatus.Suspended)
                throw new DomainConflictException(DomainErrorCodes.MerchantNotSuspended,"Askıya alınmamış merchant aktif edilemez!");

            Status = MerchantStatus.Active;
        }

        public void Delete()
        {
            if (Status == MerchantStatus.Deleted)
                throw new DomainConflictException(DomainErrorCodes.MerchantAlreadyDeleted,"Bu merchant zaten silinmiş!");

            Status = MerchantStatus.Deleted;
        }
    }
}
