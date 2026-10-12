using PSP.DOMAIN.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PSP.APPLICATION.Abstractions
{
    public interface IMerchantRepository
    {
        Task AddAsync(Merchant merchant, CancellationToken cancellationToken);
        Task<Merchant?> GetByIdAsync(Guid id , CancellationToken cancellationToken);
    }
}
