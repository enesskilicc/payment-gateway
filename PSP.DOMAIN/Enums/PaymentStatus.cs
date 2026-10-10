using System;
using System.Collections.Generic;
using System.Text;

namespace PSP.DOMAIN.Enums
{
    public enum PaymentStatus
    {
        Pending = 1,
        Authorized = 2,
        Captured = 3,
        Voided = 4,
        Refunded = 5,
        Failed = 6,
        PartiallyRefunded = 7
    }
}
