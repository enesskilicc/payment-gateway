using PSP.DOMAIN.Enums;
using PSP.DOMAIN.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

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

            Assert.Throws<ArgumentException>(() => new Payment(merchantId, amount));

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

            Assert.Throws<InvalidOperationException>(() => payment.Authorize());
        }

        [Fact]
        public void Authorize_CapturedPayment_Throws()
        {
            var payment = CreatePayment();

            payment.Authorize();
            payment.Capture();

            Assert.Throws<InvalidOperationException>(() => payment.Authorize());

        }

        [Fact]
        public void Authorize_FailedPayment_Throws()
        {
            var payment = CreatePayment();
            payment.Fail();

            Assert.Throws<InvalidOperationException>(() => payment.Authorize());
        }

        #endregion

        #region Capture Tests
        [Fact]
        public void Capture_AuthorizedPayment_StatusIsCaptured()
        {
            var payment = CreatePayment();
            payment.Authorize();

            payment.Capture();

            Assert.Equal(PaymentStatus.Captured, payment.Status);
        }

        [Fact]
        public void Capture_PendingPayment_Throws()
        {
            var payment = CreatePayment();

            Assert.Throws<InvalidOperationException>(() => payment.Capture());
        }

        [Fact]
        public void Capture_CapturedPayment_Throws()
        {
            var payment = CreatePayment();
            payment.Authorize();
            payment.Capture();

            Assert.Throws<InvalidOperationException>(() => payment.Capture());
        }

        [Fact]
        public void Capture_VoidedPayment_Throws()
        {
            var payment = CreatePayment();
            payment.Authorize();
            payment.Void();

            Assert.Throws<InvalidOperationException>(() => payment.Capture());
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

            Assert.Throws<InvalidOperationException>(() => payment.Void());
        }

        [Fact]
        public void Void_CapturedPayment_Throws()
        {
            var payment = CreatePayment();
            payment.Authorize();
            payment.Capture();

            Assert.Throws<InvalidOperationException>(() => payment.Void());
        }
        #endregion

        #region Refund Tests
        [Fact]
        public void Refund_CapturedPayment_StatusIsRefunded()
        {
            var payment = CreatePayment();
            payment.Authorize();
            payment.Capture();

            payment.Refund();

            Assert.Equal(PaymentStatus.Refunded, payment.Status);
        }

        [Fact]
        public void Refund_PendingPayment_Throws()
        {
            var payment = CreatePayment();

            Assert.Throws<InvalidOperationException>(() => payment.Refund());
        }

        [Fact]
        public void Refund_AuthorizedPayment_Throws()
        {
            var payment = CreatePayment();
            payment.Authorize();

            Assert.Throws<InvalidOperationException>(() => payment.Refund());
        }

        [Fact]
        public void Refund_RefundedPayment_Throws()
        {
            var payment = CreatePayment();
            payment.Authorize();
            payment.Capture();
            payment.Refund();

            Assert.Throws<InvalidOperationException>(() => payment.Refund());
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

            Assert.Throws<InvalidOperationException>(() => payment.Fail());
        }

        [Fact]
        public void Fail_FailedPayment_Throws()
        {
            var payment = CreatePayment();
            payment.Fail();

            Assert.Throws<InvalidOperationException>(() => payment.Fail());
        }
        #endregion
    }
}
