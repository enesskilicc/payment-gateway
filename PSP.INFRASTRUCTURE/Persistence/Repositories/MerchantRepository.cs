using Microsoft.EntityFrameworkCore;
using PSP.APPLICATION.Abstractions;
using PSP.DOMAIN.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PSP.INFRASTRUCTURE.Persistence.Repositories
{
    public sealed class MerchantRepository : IMerchantRepository
    {
        private readonly PspDbContext _context;

        public MerchantRepository(PspDbContext context)
        {
            _context = context;
        }

        public Task AddAsync(Merchant merchant, CancellationToken cancellationToken)
        {
            _context.Merchants.Add(merchant);
            return Task.CompletedTask;
        }

        public Task<Merchant?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return _context.Merchants.FirstOrDefaultAsync(x=>x.Id == id,cancellationToken);
        }
    }
}
