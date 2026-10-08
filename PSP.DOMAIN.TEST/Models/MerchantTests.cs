using PSP.DOMAIN.Enums;
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
            Assert.Throws<ArgumentException>(() => new Merchant(name));
        }
        [Fact]
        public void Constructor_Merchant_InvalidName_101Chars()
        {
            var name = new string('a', 101);

            Assert.Throws<ArgumentException>(() => new Merchant(name));
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

            Assert.Throws<InvalidOperationException>(() => merchant.Suspend());
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

            Assert.Throws<InvalidOperationException>(() => merchant.Activate());
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

            Assert.Throws<InvalidOperationException>(() => merchant.Delete());
        }

        [Fact]
        public void Activate_DeletedMerchant_Throws()
        {
            var merchant = new Merchant("Test Merchant");

            merchant.Delete();

            Assert.Throws<InvalidOperationException>(() => merchant.Activate());
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
