using PSP.DOMAIN.Enums;
using PSP.DOMAIN.Exceptions;
using PSP.DOMAIN.Models;

namespace PSP.DOMAIN.TEST.Models
{
    public class MerchantTests
    {

        [Fact]
        public void Constructor_ValidName_CreatesActiveMerchant()
        {
            var merchant = new Merchant("Test Merchant");

            Assert.Equal("Test Merchant", merchant.Name);
            Assert.Equal(MerchantStatus.Active, merchant.Status);
            Assert.NotEqual(Guid.Empty, merchant.Id);
        }


        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void Constructor_Merchant_InvalidName(string name)
        {
            var ex = Assert.Throws<DomainValidationException>(() => new Merchant(name));
            Assert.Equal(DomainErrorCodes.InvalidMerchantName, ex.Code);
        }
        [Fact]
        public void Constructor_Merchant_InvalidName_101Chars()
        {
            var name = new string('a', 101);

            var ex = Assert.Throws<DomainValidationException>(() => new Merchant(name));
            Assert.Equal(DomainErrorCodes.InvalidMerchantName, ex.Code);
        }

        [Fact]
        public void Suspend_ActiveMerchant_StatusBecomesSuspended()
        {
            var merchant = new Merchant("Test Merchant");

            merchant.Suspend();

            Assert.Equal(MerchantStatus.Suspended, merchant.Status);
        }

        [Fact]
        public void Suspend_AlreadySuspended_Throws()
        {
            var merchant = new Merchant("Test Merchant");

            merchant.Suspend();

            var ex = Assert.Throws<DomainConflictException>(() => merchant.Suspend());
            Assert.Equal(DomainErrorCodes.MerchantNotActive, ex.Code);
        }

        [Fact]
        public void Activate_SuspendMerchant_StatusBecomesActivated()
        {
            var merchant = new Merchant("Test Merchant");
            merchant.Suspend();

            merchant.Activate();

            Assert.Equal(MerchantStatus.Active, merchant.Status);
        }

        [Fact]
        public void Activate_AlreadyActivated_Throws()
        {
            var merchant = new Merchant("Test Merchant");

            var ex = Assert.Throws<DomainConflictException>(() => merchant.Activate());
            Assert.Equal(DomainErrorCodes.MerchantNotSuspended, ex.Code);
        }


        [Fact]
        public void Delete_ActiveMerchant_StatusBecomesDeleted()
        {
            var merchant = new Merchant("Test Merchant");

            merchant.Delete();

            Assert.Equal(MerchantStatus.Deleted, merchant.Status);
        }

        [Fact]
        public void Delete_AlreadyDeleted_Throws()
        {
            var merchant = new Merchant("Test Merchant");

            merchant.Delete();

            var ex = Assert.Throws<DomainConflictException>(() => merchant.Delete());
            Assert.Equal(DomainErrorCodes.MerchantAlreadyDeleted, ex.Code);
        }

        [Fact]
        public void Activate_DeletedMerchant_Throws()
        {
            var merchant = new Merchant("Test Merchant");

            merchant.Delete();

            var ex = Assert.Throws<DomainConflictException>(() => merchant.Activate());
            Assert.Equal(DomainErrorCodes.MerchantNotSuspended, ex.Code);
        }

        [Fact]
        public void Delete_SuspendedMerchant_StatusBecomesDeleted()
        {
            var merchant = new Merchant("Test Merchant");

            merchant.Suspend();

            merchant.Delete();

            Assert.Equal(MerchantStatus.Deleted, merchant.Status);
        }
    }
}
