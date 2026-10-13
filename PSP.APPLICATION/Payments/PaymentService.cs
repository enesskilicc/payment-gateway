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
        private readonly IPaymentProcessor _paymentProcessor;

        public PaymentService(IPaymentRepository paymentRepository, IUnitOfWork unitOfWork, IMerchantRepository merchantRepository,IPaymentProcessor paymentProcessor)
        {
            _paymentRepository = paymentRepository;
            _unitOfWork = unitOfWork;
            _merchantRepository = merchantRepository;
            _paymentProcessor = paymentProcessor;
        }

        public async Task<PaymentResponse> CreateAsync(Guid merchantId, CreatePaymentRequest request, CancellationToken cancellationToken)
        {
            var merchant = await _merchantRepository.GetByIdAsync(merchantId, cancellationToken) ?? throw new InvalidOperationException($"Doğrulanmış merchant bulunamadı: {merchantId}");
            merchant.EnsureActive();
            var money = new Money(request.Amount, request.Currency);
            var payment = new Payment(merchant.Id, money,request.MerchantOrderId);
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

        public async Task<PaymentResponse?> ConfirmAsync(Guid id, Guid merchantId, CancellationToken cancellationToken)
        {
            var merchant = await _merchantRepository.GetByIdAsync(merchantId, cancellationToken) ?? throw new InvalidOperationException($"Doğrulanmış merchant bulunamadı: {merchantId}");


            merchant.EnsureActive();

            var payment = await _paymentRepository.GetByIdAsync(id,merchantId,cancellationToken);

            if (payment is null)
                return null;

            payment.EnsureConfirmable();

            var authorizeResult = await _paymentProcessor.AuthorizeAsync(payment,cancellationToken);

            if (authorizeResult.Approved)
                payment.Authorize();

            else
                payment.Fail();

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return PaymentResponse.FromDomain(payment);
        }


    }
}
