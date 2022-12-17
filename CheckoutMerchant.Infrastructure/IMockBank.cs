using CheckoutMerchant.Models;
using MockBankClient;

namespace CheckoutMerchant.Infrastructure
{
    public interface IMockBank
    {
        Task<PaymentInitationResponse> InitiatePaymentAsync(ProcessPaymentRequest request, TokenInfo tokenInfo);
        
        Task<PaymentResponse> ProcessPaymentAsync(PaymentInitationResponse paymentInitiationResponse, TokenInfo tokenInfo);

        Task<PaymentResponse> GetPaymentDetailsAsync(PaymentIdRequest paymentRequest, TokenInfo tokenInfo);
    }
}
