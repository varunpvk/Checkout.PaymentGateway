using AutoFixture;
using CheckoutMerchant.Infrastructure;
using CheckoutMerchant.Models;
using CheckoutMerchantAPI.Controllers;
using FluentValidation;
using FluentValidation.Results;
using MockBankClient;
using Moq;

namespace CheckoutMerchant.API.Tests
{
    public class PaymentsControllerTests
    {
        private readonly Mock<IMockBank> _mockBankMock;
        private readonly Mock<IMerchantService> _merchantServiceMock;
        private readonly Mock<IValidator<ProcessPaymentRequest>> _processPaymentRequestValidatorMock;
        private readonly Mock<IValidator<MerchantPaymentDetails>> _merchantPaymentDetailsValidatorMock;
        private readonly PaymentsController _paymentsController;

        public PaymentsControllerTests()
        {
            _mockBankMock = new Mock<IMockBank>();
            _merchantServiceMock = new Mock<IMerchantService>();
            _processPaymentRequestValidatorMock = new Mock<IValidator<ProcessPaymentRequest>>();
            _merchantPaymentDetailsValidatorMock = new Mock<IValidator<MerchantPaymentDetails>>();
            _paymentsController = new PaymentsController(
                _mockBankMock.Object, 
                _merchantServiceMock.Object, 
                _processPaymentRequestValidatorMock.Object, 
                _merchantPaymentDetailsValidatorMock.Object);
        }

        [Fact]
        public async Task Successfully_Processes_A_PaymentAsync()
        {
            //arrange
            var _fixture = new Fixture();
            var _tokenInfo = _fixture.Create<TokenInfo>();
            var _paymentInitiationResponse = _fixture.Create<PaymentInitationResponse>();
            var _paymentResponse = _fixture.Create<PaymentResponse>();
            var _processPaymentRequest = _fixture.Create<ProcessPaymentRequest>();

            _processPaymentRequestValidatorMock
                .Setup(o => o.ValidateAsync(It.IsAny<ProcessPaymentRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult { Errors = new List<ValidationFailure>() });
            _merchantServiceMock
                .Setup(o => o.GetAccessTokenAsync(It.IsAny<string>()))
                .ReturnsAsync(_tokenInfo);
            _mockBankMock
                .Setup(o => o.InitiatePaymentAsync(It.IsAny<ProcessPaymentRequest>(), _tokenInfo))
                .ReturnsAsync(_paymentInitiationResponse);
            _mockBankMock
                .Setup(o => o.ProcessPaymentAsync(_paymentInitiationResponse, _tokenInfo))
                .ReturnsAsync(_paymentResponse);

            //act
            var result = await _paymentsController.ProcessPaymentAsync(_processPaymentRequest);

            //assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task Should_Return_SuccessfulPaymentAsync()
        {
            //arrange
            var _fixture = new Fixture();
            var _tokenInfo = _fixture.Create<TokenInfo>();
            var _paymentResposne = _fixture.Create<PaymentResponse>();
            var _merchantPaymentDetails = _fixture.Create<MerchantPaymentDetails>();

            _merchantPaymentDetailsValidatorMock
                .Setup(o => o.ValidateAsync(It.IsAny<MerchantPaymentDetails>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult { Errors = new List<ValidationFailure>() });
            _merchantServiceMock
                .Setup(o => o.GetAccessTokenAsync(It.IsAny<string>()))
                .ReturnsAsync(_tokenInfo);
            _mockBankMock
                .Setup(o => o.GetPaymentDetailsAsync(It.IsAny<PaymentIdRequest>(), _tokenInfo))
                .ReturnsAsync(_paymentResposne);

            //act
            var result = await _paymentsController.GetPaymentAsync(_merchantPaymentDetails);

            //assert
            Assert.NotNull(result);
        }

    }
}
