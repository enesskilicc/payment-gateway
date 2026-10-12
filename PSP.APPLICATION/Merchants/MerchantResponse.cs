using PSP.DOMAIN.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PSP.APPLICATION.Merchants
{
    public sealed record MerchantResponse(Guid Id,string Name,string Status,DateTimeOffset CreatedAt)
    {
        public static MerchantResponse FromDomain(Merchant merchant)
        {
            return new MerchantResponse(merchant.Id,merchant.Name,merchant.Status.ToString(),merchant.CreatedAt);
        }
    }
}
