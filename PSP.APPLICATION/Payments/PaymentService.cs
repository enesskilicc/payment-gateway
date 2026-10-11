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

        public PaymentService(IPaymentRepository paymentRepository, IUnitOfWork unitOfWork)
        {
            _paymentRepository = paymentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<PaymentResponse> CreateAsync(Guid merchantId, CreatePaymentRequest request, CancellationToken cancellationToken)
        {
            var money = new Money(request.Amount, request.Currency);
            var payment = new Payment(merchantId, money);
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
