using Microsoft.EntityFrameworkCore;
using PSP.APPLICATION.Abstractions;
using PSP.DOMAIN.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PSP.INFRASTRUCTURE.Persistence.Repositories
{
    public sealed class PaymentRepository : IPaymentRepository
    {
        private readonly PspDbContext _context;

        public PaymentRepository(PspDbContext context)
        {
            _context = context;
        }

        public Task AddAsync(Payment payment, CancellationToken cancellationToken)
        {
            _context.Payments.Add(payment);
            return Task.CompletedTask;
        }

        public Task<Payment?> GetByIdAsync(Guid id, Guid merchantId, CancellationToken cancellationToken)
        {
            return _context.Payments.FirstOrDefaultAsync(x=>x.Id == id && x.MerchantId == merchantId,cancellationToken);
        }
    }
}
