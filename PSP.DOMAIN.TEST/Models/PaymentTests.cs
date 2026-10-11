using PSP.DOMAIN.Enums;
using PSP.DOMAIN.Exceptions;
using PSP.DOMAIN.Models;

namespace PSP.DOMAIN.TEST.Models
{
    public class PaymentTests
    {
        private static Payment CreatePayment()
        {
            var merchantId = Guid.NewGuid();
            var amount = new Money(100, "TRY");
            var payment = new Payment(merchantId, amount);

            return payment;
        }
        [Theory]
        [InlineData(0)]
        [InlineData(-100)]
        public void Constructor_NonPositiveAmount_Throws(long minorUnit)
        {
            var merchantId = Guid.NewGuid();
            var amount = new Money(minorUnit, "TRY");

            var ex = Assert.Throws<DomainValidationException>(() => new Payment(merchantId, amount));
            Assert.Equal(DomainErrorCodes.InvalidAmount, ex.Code);

        }

        [Fact]
        public void Constructor_NullAmount_Throws()
        {
            var merchantId = Guid.NewGuid();

            Assert.Throws<ArgumentNullException>(() => new Payment(merchantId, null!));
        }


        [Fact]
        public void Constructor_ValidInput_CreatesPendingPayment()
        {
            var merchantId = Guid.NewGuid();
            var amount = new Money(100, "TRY");
            var payment = new Payment(merchantId, amount);

            Assert.NotEqual(Guid.Empty, payment.Id);
            Assert.Equal(PaymentStatus.Pending, payment.Status);
            Assert.Equal(merchantId, payment.MerchantId);
            Assert.Equal(amount, payment.Amount);
        }

        #region Authorize Tests
        [Fact]
        public void Authorize_PendingPayment_StatusIsAuthorized()
        {
            var payment = CreatePayment();

            payment.Authorize();

            Assert.Equal(PaymentStatus.Authorized, payment.Status);
        }

        [Fact]
        public void Authorize_AuthorizedPayment_Throws()
        {
            var payment = CreatePayment();
            payment.Authorize();

            var ex = Assert.Throws<DomainConflictException>(() => payment.Authorize());
            Assert.Equal(DomainErrorCodes.PaymentNotAuthorizable, ex.Code);
        }

        [Fact]
        public void Authorize_CapturedPayment_Throws()
        {
            var payment = CreatePayment();

            payment.Authorize();
            payment.Capture(payment.Amount);

            var ex = Assert.Throws<DomainConflictException>(() => payment.Authorize());
            Assert.Equal(DomainErrorCodes.PaymentNotAuthorizable, ex.Code);

        }

        [Fact]
        public void Authorize_FailedPayment_Throws()
        {
            var payment = CreatePayment();
            payment.Fail();

            var ex = Assert.Throws<DomainConflictException>(() => payment.Authorize());
            Assert.Equal(DomainErrorCodes.PaymentNotAuthorizable, ex.Code);
        }

        #endregion

        #region Capture Tests
        [Fact]
        public void Capture_AuthorizedPayment_StatusIsCaptured()
        {
            var payment = CreatePayment();
            payment.Authorize();

            payment.Capture(payment.Amount);

            Assert.Equal(PaymentStatus.Captured, payment.Status);
        }

        [Fact]
        public void Capture_PendingPayment_Throws()
        {
            var payment = CreatePayment();

            var ex = Assert.Throws<DomainConflictException>(() => payment.Capture(payment.Amount));
            Assert.Equal(DomainErrorCodes.PaymentNotCapturable, ex.Code);
        }

        [Fact]
        public void Capture_CapturedPayment_Throws()
        {
            var payment = CreatePayment();
            payment.Authorize();
            payment.Capture(payment.Amount);

            var ex = Assert.Throws<DomainConflictException>(() => payment.Capture(payment.Amount));
            Assert.Equal(DomainErrorCodes.PaymentNotCapturable, ex.Code);
        }

        [Fact]
        public void Capture_VoidedPayment_Throws()
        {
            var payment = CreatePayment();
            payment.Authorize();
            payment.Void();

            var ex = Assert.Throws<DomainConflictException>(() => payment.Capture(payment.Amount));
            Assert.Equal(DomainErrorCodes.PaymentNotCapturable, ex.Code);
        }
        #endregion

        #region Void Tests
        [Fact]
        public void Void_AuthorizedPayment_StatusIsVoided()
        {
            var payment = CreatePayment();
            payment.Authorize();

            payment.Void();

            Assert.Equal(PaymentStatus.Voided, payment.Status);
        }

        [Fact]
        public void Void_PendingPayment_Throws()
        {
            var payment = CreatePayment();

            var ex = Assert.Throws<DomainConflictException>(() => payment.Void());
            Assert.Equal(DomainErrorCodes.PaymentNotVoidable, ex.Code);
        }

        [Fact]
        public void Void_CapturedPayment_Throws()
        {
            var payment = CreatePayment();
            payment.Authorize();
            payment.Capture(payment.Amount);

            var ex = Assert.Throws<DomainConflictException>(() => payment.Void());
            Assert.Equal(DomainErrorCodes.PaymentNotVoidable, ex.Code);
        }
        #endregion

        #region Refund Tests
        [Fact]
        public void Refund_CapturedPayment_StatusIsRefunded()
        {
            var payment = CreatePayment();
            payment.Authorize();
            payment.Capture(payment.Amount);

            payment.Refund(payment.Amount);

            Assert.Equal(PaymentStatus.Refunded, payment.Status);
        }

        [Fact]
        public void Refund_PendingPayment_Throws()
        {
            var payment = CreatePayment();

            var ex = Assert.Throws<DomainConflictException>(() => payment.Refund(payment.Amount));
            Assert.Equal(DomainErrorCodes.PaymentNotRefundable, ex.Code);
        }

        [Fact]
        public void Refund_AuthorizedPayment_Throws()
        {
            var payment = CreatePayment();
            payment.Authorize();

            var ex = Assert.Throws<DomainConflictException>(() => payment.Refund(payment.Amount));
            Assert.Equal(DomainErrorCodes.PaymentNotRefundable, ex.Code);
        }

        [Fact]
        public void Refund_RefundedPayment_Throws()
        {
            var payment = CreatePayment();
            payment.Authorize();
            payment.Capture(payment.Amount);
            payment.Refund(payment.Amount);

            var ex = Assert.Throws<DomainConflictException>(() => payment.Refund(payment.Amount));
            Assert.Equal(DomainErrorCodes.PaymentNotRefundable, ex.Code);
        }
        #endregion

        #region Fail Tests
        [Fact]
        public void Fail_PendingPayment_StatusIsFailed()
        {
            var payment = CreatePayment();

            payment.Fail();

            Assert.Equal(PaymentStatus.Failed, payment.Status);
        }

        [Fact]
        public void Fail_AuthorizedPayment_Throws()
        {
            var payment = CreatePayment();
            payment.Authorize();

            var ex = Assert.Throws<DomainConflictException>(() => payment.Fail());
            Assert.Equal(DomainErrorCodes.PaymentAlreadyProcessed, ex.Code);
        }

        [Fact]
        public void Fail_FailedPayment_Throws()
        {
            var payment = CreatePayment();
            payment.Fail();

            var ex = Assert.Throws<DomainConflictException>(() => payment.Fail());
            Assert.Equal(DomainErrorCodes.PaymentAlreadyProcessed, ex.Code);
        }
        #endregion

        [Fact]
        public void Capture_PartialAmount_SetsCapturedAmount()
        {
            var merchantId = Guid.NewGuid();
            var amount = new Money(100, "TRY");
            var payment = new Payment(merchantId, amount);
            payment.Authorize();
            payment.Capture(new Money(60, "TRY"));

            Assert.Equal(PaymentStatus.Captured, payment.Status);
        }


        [Fact]
        public void Capture_AmountExceedsAuthorized_Throws()
        {
            var merchantId = Guid.NewGuid();
            var amount = new Money(60, "TRY");
            var payment = new Payment(merchantId, amount);

            var newAmount = new Money(150, "TRY");
            payment.Authorize();
            var ex = Assert.Throws<DomainValidationException>(() => payment.Capture(newAmount));
            Assert.Equal(DomainErrorCodes.AmountExceedsAuthorized, ex.Code);
        }

        [Fact]
        public void Refund_PartialAmount_StatusIsPartiallyRefunded()
        {
            var merchantId = Guid.NewGuid();
            var amount = new Money(100, "TRY");
            var payment = new Payment(merchantId, amount);
            payment.Authorize();
            payment.Capture(amount);

            var refundAmount = new Money(30, "TRY");

            payment.Refund(refundAmount);

            Assert.Equal(PaymentStatus.PartiallyRefunded, payment.Status);
            Assert.Equal(payment.RefundedAmount, refundAmount);

        }

        [Fact]
        public void Refund_RemainingAmount_StatusIsRefunded()
        {
            var merchantId = Guid.NewGuid();
            var amount = new Money(100, "TRY");
            var payment = new Payment(merchantId, amount);
            payment.Authorize();
            payment.Capture(amount);
            var ref1 = new Money(30, "TRY");
            var ref2 = new Money(70, "TRY");

            payment.Refund(ref1);
            payment.Refund(ref2);

            Assert.Equal(PaymentStatus.Refunded, payment.Status);
        }

        [Fact]
        public void Refund_AmountExceedsRefundable_Throws()
        {
            var merchantId = Guid.NewGuid();
            var amount = new Money(100, "TRY");
            var payment = new Payment(merchantId, amount);
            payment.Authorize();
            payment.Capture(amount);

            payment.Refund(new Money(30, "TRY"));

            var ex = Assert.Throws<DomainValidationException>(() => payment.Refund(new Money(80, "TRY")));
            Assert.Equal(DomainErrorCodes.AmountExceedsRefundable, ex.Code);
        }
    }
}
