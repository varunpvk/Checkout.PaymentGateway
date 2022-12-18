using CheckoutMerchant.Infrastructure;
using CheckoutMerchant.Models;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using MockBankClient;

namespace CheckoutMerchantAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IMockBank _mockBank;
        private readonly IMerchantService _merchantService;
        private readonly IValidator<ProcessPaymentRequest> _processPaymentRequestValidator;
        private readonly IValidator<MerchantPaymentDetails> _merchantPaymentDetailsValidator;

        public PaymentsController(
            IMockBank mockBank,
            IMerchantService merchantService,
            IValidator<ProcessPaymentRequest> processPaymentRequestValidator,
            IValidator<MerchantPaymentDetails> merchantPaymentDetailsValidator)
        {
            _mockBank = mockBank;
            _merchantService = merchantService;
            _processPaymentRequestValidator = processPaymentRequestValidator;
            _merchantPaymentDetailsValidator = merchantPaymentDetailsValidator; 
        }

        [HttpPost(Name = "processpayment")]
        public async Task<ProcessPaymentResponse> ProcessPaymentAsync(ProcessPaymentRequest paymentRequest)
        {
            ValidationResult result = await _processPaymentRequestValidator.ValidateAsync(paymentRequest);

            if(!result.IsValid)
            {
                throw new ArgumentException($"Validation Error: {result.Errors.First().ErrorMessage}");
            }
            //call initiatePayment to generate a consentid from bank
            //redirect the consentid to shopper for authentication
            //get authcode from bank
            //exchange authcode with access token with a timelimit to expiry
            //use the authtoken to process a payment
            //We assume the enduser has given his consent to process the payment to the Merchant and hence the flow will be done as given below
            //  1: Request AccessToken
            var tokenInfo = await _merchantService.GetAccessTokenAsync(paymentRequest.Initiation.InstructionIdentifier);

            //  2: Initate Payment
            var initPayment = await _mockBank.InitiatePaymentAsync(paymentRequest, tokenInfo);

            ////  3: Process Payment
            var paymentResponse = await _mockBank.ProcessPaymentAsync(initPayment, tokenInfo);

            //return processPaymentResponse;
            return new ProcessPaymentResponse
            {
                ConsentId = paymentResponse.ConsentId,
                PaymentId = paymentResponse.PaymentId,
                Initiation = new CheckoutMerchant.Models.Initiation
                {
                    InstructedAmount = new CheckoutMerchant.Models.InstructedAmount
                    {
                        Amount = paymentResponse.Initiation.InstructedAmount.Amount,
                        Currency = paymentResponse.Initiation.InstructedAmount.Currency
                    },
                    InstructionIdentifier = paymentResponse.Initiation.InstructionIdentifier,
                },
                CreditorAccount = new CheckoutMerchant.Models.CreditorAccount
                {
                    CardNumber = paymentResponse.CreditorAccount.CardNumber,
                    ExpiryDate = paymentResponse.CreditorAccount.ExpiryDate,
                    Cvv = paymentResponse.CreditorAccount.Cvv
                },
                CreatedDateTime = paymentResponse.CreatedDateTime,
                StatusUpdateDateTime = paymentResponse.StatusUpdateDateTime,
                Status = paymentResponse.Status,
                PaymentStatusLink = new CheckoutMerchant.Models.Link
                {
                    Url = paymentResponse.PaymentStatusLink.Url
                }
            };
        }

        [HttpGet]
        public async Task<ProcessPaymentResponse> GetPaymentAsync([FromQuery]MerchantPaymentDetails merchantPaymentDetails)
        {
            ValidationResult result = await _merchantPaymentDetailsValidator.ValidateAsync(merchantPaymentDetails);

            if (!result.IsValid)
            {
                throw new ArgumentException($"Validation Error: {result.Errors.First().ErrorMessage}");
            }

            var tokenInfo = await _merchantService.GetAccessTokenAsync(merchantPaymentDetails.InstructionIdentifier);

            var paymentResponse = await _mockBank.GetPaymentDetailsAsync(new PaymentIdRequest
            {
                PaymentId = merchantPaymentDetails.PaymentId
            },
            tokenInfo);

            return new ProcessPaymentResponse
            {
                ConsentId = paymentResponse.ConsentId,
                PaymentId = paymentResponse.PaymentId,
                Initiation = new CheckoutMerchant.Models.Initiation
                {
                    InstructedAmount = new CheckoutMerchant.Models.InstructedAmount
                    {
                        Amount = paymentResponse.Initiation.InstructedAmount.Amount,
                        Currency = paymentResponse.Initiation.InstructedAmount.Currency
                    },
                    InstructionIdentifier = paymentResponse.Initiation.InstructionIdentifier
                },
                CreditorAccount = new CheckoutMerchant.Models.CreditorAccount 
                {
                    CardNumber = paymentResponse.CreditorAccount.CardNumber,
                    ExpiryDate = paymentResponse.CreditorAccount.ExpiryDate,
                    Cvv = paymentResponse.CreditorAccount.Cvv
                },
                CreatedDateTime = paymentResponse.CreatedDateTime,
                StatusUpdateDateTime = paymentResponse.StatusUpdateDateTime,
                Status = paymentResponse.Status
            };
        }
    }
}
