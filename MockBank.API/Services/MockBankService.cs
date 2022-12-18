using Grpc.Core;
using PaymentsRepository;
using System.Text.Json;

namespace MockBankAPI.Services
{
    public class MockBankService : MockBank.MockBankBase
    {
        private readonly IPaymentsRepository _paymentsRepository;

        public MockBankService(IPaymentsRepository paymentsRepository)
        {
            _paymentsRepository = paymentsRepository;
        }
        
        public override async Task<PaymentInitationResponse> InitiatePayment(InitiatePaymentRequest request, ServerCallContext context)
        {
            //Verify for a valid accessToken - for now we shall rely only expiry.
            var bearerToken = context.GetHttpContext().Request.Headers["Authorization"];
            long.TryParse(context.GetHttpContext().Request.Headers["ExpiresIn"], out var expiresIn);

            bool isAccessTokenExpired = DateTime.UtcNow > new DateTime(expiresIn);

            if (isAccessTokenExpired)
                throw new RpcException(new Status(StatusCode.PermissionDenied, "AccessToken expired"));

            var jsonObject = JsonSerializer.Serialize(request, new JsonSerializerOptions
            {
                WriteIndented = true,
            });

            var consentId = Guid.NewGuid().ToString();

            var paymentInitiationResponse = new PaymentInitationResponse
            {
                PaymentId = request.PaymentId,
                ConsentId = consentId, //Get Consent
                Status = "AwaitingAuthorisation",
                CreationDateTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                StatusUpdateDateTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                Initiation = request.Initiation,
                CreditorAccount = request.CreditorAccount,
                RedirectLink = new Link
                {
                    Url = $"https://localhost:7240/open-banking/v3.1/pisp/initiate-payment-consent/{consentId}"
                }
            };

            var paymentInsertSuccess = await _paymentsRepository.CreatePaymentAsync(new PaymentsRepository.PaymentModels.PaymentRequestDetails
            {
                PaymentId = request.PaymentId,
                ConsentId = consentId,
                PaymentDetails = JsonSerializer.Serialize(request),
                PaymentStatus = "AwaitingAuthorisation",
                CreatedDate = DateTime.UtcNow,
                LastUpdatedDate = DateTime.UtcNow
            });

            var user = "Varun";//context.GetHttpContext().User;
            var metadata = new Metadata
            {
                { "User", user }
            };

            if (paymentInsertSuccess)
                return paymentInitiationResponse;
            else
                throw new RpcException(new Status(StatusCode.Aborted, "Operation completed unsuccessfully"), metadata);
        }

        public override async Task<PaymentResponse> ProcessPayment(PaymentRequest request, ServerCallContext context)
        {
            //Verify for a valid accessToken - for now we shall rely only expiry.
            var bearerToken = context.GetHttpContext().Request.Headers["Authorization"];
            long.TryParse(context.GetHttpContext().Request.Headers["ExpiresIn"], out var expiresIn);

            bool isAccessTokenExpired = DateTime.UtcNow > new DateTime(expiresIn);

            if (isAccessTokenExpired)
                throw new RpcException(new Status(StatusCode.PermissionDenied, "AccessToken expired"));

            bool isPaymentConsentValid = true; //ValidateConsent(consentId);

            var user = "Varun";//context.GetHttpContext().User;
            var metadata = new Metadata
                {
                    { "User", user }
                };

            if (isPaymentConsentValid)
            {
                var paymentResponse = new PaymentResponse
                {
                    ConsentId = request.ConsentId,
                    CreatedDateTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                    CreditorAccount = request.CreditorAccount,
                    Initiation = request.Initiation,
                    PaymentId = request.PaymentId,
                    Status = "AcceptedSettlementInProcess",
                    StatusUpdateDateTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                    PaymentStatusLink = new Link
                    {
                        Url = $"https://localhost:7240/open-banking/v3.1/pisp/process-payment/{request.PaymentId}"
                    }
                };

                var processPaymentSuccess = await _paymentsRepository.UpdatePaymentAsync(new PaymentsRepository.PaymentModels.PaymentRequestDetails
                {
                    PaymentId = request.PaymentId,
                    ConsentId = request.ConsentId,
                    PaymentDetails = JsonSerializer.Serialize(request),
                    PaymentStatus = "AcceptedSettlementInProcess",
                    LastUpdatedDate = DateTime.UtcNow
                });

                if (processPaymentSuccess)
                    return paymentResponse;
                else
                    throw new RpcException(new Status(StatusCode.Aborted, "Operation completed unsuccessfully"), metadata);
            }

            throw new RpcException(new Status(StatusCode.PermissionDenied, "Permission Denied"), metadata);
        }

        public override async Task<PaymentResponse> GetPayment(PaymentIdRequest request, ServerCallContext context)
        {
            var bearerToken = context.GetHttpContext().Request.Headers["Authorization"];
            long.TryParse(context.GetHttpContext().Request.Headers["ExpiresIn"], out var expiresIn);

            bool isAccessTokenExpired = DateTime.UtcNow > new DateTime(expiresIn);

            if (isAccessTokenExpired)
                throw new RpcException(new Status(StatusCode.PermissionDenied, "AccessToken expired"));

            var paymentResponse = await _paymentsRepository.GetPaymentAsync(request.PaymentId);

            var paymentInfo = JsonSerializer.Deserialize<PaymentRequest>(paymentResponse.PaymentDetails);

            if(paymentResponse != null)
            {
                return new PaymentResponse
                {
                    ConsentId = paymentResponse.ConsentId,
                    PaymentId = paymentResponse.PaymentId,
                    Initiation = paymentInfo.Initiation,
                    Status = paymentResponse.PaymentStatus,
                    CreatedDateTime = paymentResponse.CreatedDate.ToString(),
                    StatusUpdateDateTime = paymentResponse.LastUpdatedDate.ToString(),
                    CreditorAccount = paymentInfo.CreditorAccount
                };
            }

            var user = "Varun";//context.GetHttpContext().User;
            var metadata = new Metadata
                {
                    { "User", user }
                };

            throw new RpcException(new Status(StatusCode.Aborted, "Operation completed unsuccessfully"), metadata);
        }
    }
}