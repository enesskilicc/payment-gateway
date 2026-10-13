using System;
using System.Collections.Generic;
using System.Text;

namespace PSP.DOMAIN.Exceptions
{
    public static class DomainErrorCodes
    {
        // Money
        public const string InvalidCurrency = "invalid_currency";
        public const string CurrencyMismatch = "currency_mismatch";

        // Payment: girdi
        public const string InvalidAmount = "invalid_amount";
        public const string AmountExceedsAuthorized = "amount_exceeds_authorized";
        public const string AmountExceedsRefundable = "amount_exceeds_refundable";
        public const string InvalidMerchantOrderId = "invalid_merchant_order_id";

        // Payment: durum
        public const string PaymentNotAuthorizable = "payment_not_authorizable";
        public const string PaymentNotCapturable = "payment_not_capturable";
        public const string PaymentNotVoidable = "payment_not_voidable";
        public const string PaymentNotRefundable = "payment_not_refundable";
        public const string PaymentAlreadyProcessed = "payment_already_processed";
        
        // Merchant
        public const string InvalidMerchantName = "invalid_merchant_name";
        public const string MerchantNotActive = "merchant_not_active";
        public const string MerchantNotSuspended = "merchant_not_suspended";
        public const string MerchantAlreadyDeleted = "merchant_already_deleted";
        

        // ApiKey
        public const string ApiKeyAlreadyRevoked = "api_key_already_revoked";
    }
}
