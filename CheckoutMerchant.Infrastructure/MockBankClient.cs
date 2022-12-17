using CheckoutMerchant.Models;
using Grpc.Core;
using Grpc.Net.Client;
using MockBankClient;
using CreditorAccount = MockBankClient.CreditorAccount;
using Initiation = MockBankClient.Initiation;
using InstructedAmount = MockBankClient.InstructedAmount;

namespace CheckoutMerchant.Infrastructure
{
    public class MockBankClient : IMockBank
    {
        private readonly GrpcChannel _channel;
        private readonly MockBank.MockBankClient _client;

        public MockBankClient()
        {
            _channel = GrpcChannel.ForAddress("http://MockBank.API");
            _client = new MockBank.MockBankClient(_channel);
        }

        public async Task<PaymentResponse> GetPaymentDetailsAsync(PaymentIdRequest paymentRequest, TokenInfo tokenInfo)
        {
            var paymentResponse = await _client.GetPaymentAsync(paymentRequest, AddHeaders(tokenInfo));

            return paymentResponse;
        }

        public async Task<PaymentInitationResponse> InitiatePaymentAsync(ProcessPaymentRequest request, TokenInfo tokenInfo)
        {
            var paymentInitiationRequest = new InitiatePaymentRequest
            {
                CreditorAccount = new CreditorAccount
                {
                    CardNumber = request.CreditorAccount.CardNumber,
                    Cvv = request.CreditorAccount.Cvv,
                    ExpiryDate = request.CreditorAccount.ExpiryDate
                },
                Initiation = new Initiation
                {
                    InstructionIdentifier = request.Initiation.InstructionIdentifier,
                    InstructedAmount = new InstructedAmount
                    {
                        Amount = request.Initiation.InstructedAmount.Amount,
                        Currency = request.Initiation.InstructedAmount.Currency
                    }
                },
                PaymentId = Guid.NewGuid().ToString()
            };

            var paymentResponse = await _client.InitiatePaymentAsync(paymentInitiationRequest, AddHeaders(tokenInfo));

            if (paymentResponse != null && Guid.TryParse(paymentResponse.PaymentId, out var paymentId))
                return paymentResponse;

            throw new InvalidOperationException("Failed to initiate Payment");
        }

        public async Task<PaymentResponse> ProcessPaymentAsync(PaymentInitationResponse paymentInitiationResponse, TokenInfo tokenInfo)
        {
            var processPaymentRequest = new PaymentRequest
            {
                ConsentId = paymentInitiationResponse.ConsentId,
                CreditorAccount = paymentInitiationResponse.CreditorAccount,
                Initiation = paymentInitiationResponse.Initiation,
                PaymentId = paymentInitiationResponse.PaymentId
            };

            var processPayment = await _client.ProcessPaymentAsync(processPaymentRequest, AddHeaders(tokenInfo));

            if (processPayment != null)
            {
                return processPayment;
            }

            throw new InvalidOperationException("Failed to process Payment");
        }

        private Metadata AddHeaders(TokenInfo tokenInfo) => new Metadata()
            {
                { "Authorization", tokenInfo.AccessToken },
                { "ExpiresIn", tokenInfo.ExpiresIn.ToString() }
            };
    }
}
