using PaymentsRepository.PaymentModels;

namespace PaymentsRepository
{
    public interface IPaymentsRepository
    {
        Task<bool> CreatePaymentAsync(PaymentRequestDetails paymentDetails);
        Task<bool> UpdatePaymentAsync(PaymentRequestDetails paymentDetails);
        Task<PaymentRequestDetails> GetPaymentAsync(string paymentId);
    }
}
