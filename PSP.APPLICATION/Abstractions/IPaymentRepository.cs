using PSP.DOMAIN.Models;

namespace PSP.APPLICATION.Abstractions
{
    public interface IPaymentRepository
    {
        Task AddAsync(Payment payment, CancellationToken cancellationToken);
        Task<Payment?> GetByIdAsync(Guid id, Guid merchantId, CancellationToken cancellationToken);
    }
}
