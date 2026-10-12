using PSP.APPLICATION.Abstractions;
using PSP.DOMAIN.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PSP.APPLICATION.Payments
{
    public sealed class PaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMerchantRepository _merchantRepository;

        public PaymentService(IPaymentRepository paymentRepository, IUnitOfWork unitOfWork, IMerchantRepository merchantRepository)
        {
            _paymentRepository = paymentRepository;
            _unitOfWork = unitOfWork;
            _merchantRepository = merchantRepository;
        }

        public async Task<PaymentResponse> CreateAsync(Guid merchantId, CreatePaymentRequest request, CancellationToken cancellationToken)
        {
            var merchant = await _merchantRepository.GetByIdAsync(merchantId, cancellationToken) ?? throw new InvalidOperationException($"Doğrulanmış merchant bulunamadı: {merchantId}");
            merchant.EnsureActive();
            var money = new Money(request.Amount, request.Currency);
            var payment = new Payment(merchant.Id, money);
            await _paymentRepository.AddAsync(payment, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return PaymentResponse.FromDomain(payment);
        }


        public async Task<PaymentResponse?> GetByIdAsync(Guid id, Guid merchantId, CancellationToken cancellationToken)
        {
            var payment = await _paymentRepository.GetByIdAsync(id, merchantId, cancellationToken);

            if (payment is null)
                return null;

            return PaymentResponse.FromDomain(payment);
        }
    }
}
