using AutoFixture;
using CheckoutMerchant.API.Controllers;
using CheckoutMerchant.Infrastructure;
using CheckoutMerchant.Models;
using CheckoutMerchant.Models.Merchant;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace CheckoutMerchant.API.Tests
{
    public class MerchantControllerTests
    {
        private readonly Mock<IMerchantService> _merchantServiceMock;
        private readonly Mock<IMockBankAuth> _mockBankAuthMock;
        private readonly Mock<IValidator<MerchantDetails>> _validatorMock;

        private readonly MerchantController _merchantController;
        

        public MerchantControllerTests()
        {
            _merchantServiceMock = new Mock<IMerchantService>();
            _mockBankAuthMock = new Mock<IMockBankAuth>();
            _validatorMock = new Mock<IValidator<MerchantDetails>>();

            _merchantController = new MerchantController(_merchantServiceMock.Object, _mockBankAuthMock.Object, _validatorMock.Object);
        }

        [Fact]
        public async Task Successfully_Creates_A_Merchant()
        {
            //arrange
            var _fixture = new Fixture();
            var accessToken = _fixture.Create<MockBankClient.AccessToken>();
            var merchantDetails = _fixture.Create<MerchantDetails>();
            _validatorMock.Setup(x => x.ValidateAsync(It.IsAny<MerchantDetails>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult { Errors = new List<ValidationFailure>() });
            _mockBankAuthMock.Setup(o => o.GenerateClientCredentialsAsync(It.IsAny<MerchantDetails>())).ReturnsAsync(accessToken);
            _merchantServiceMock.Setup(o => o.UpsertMerchantDetailsAsync(It.IsAny<MerchantDetails>(), It.IsAny<TokenInfo>())).ReturnsAsync(true);

            //act
            var result = await _merchantController.CreateMerchantAsync(merchantDetails);

            //assert
            Assert.NotNull(result);
            result.Equals(accessToken.Token);
        }

        [Fact]
        public async Task Exception_While_UpsertMerchantDetailsAsync_Throws_Exception()
        {
            //arrange
            var _fixture = new Fixture();
            var accessToken = _fixture.Create<MockBankClient.AccessToken>();
            var merchantDetails = _fixture.Create<MerchantDetails>();
            _validatorMock.Setup(x => x.ValidateAsync(It.IsAny<MerchantDetails>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult { Errors = new List<ValidationFailure>() });
            _mockBankAuthMock.Setup(o => o.GenerateClientCredentialsAsync(It.IsAny<MerchantDetails>())).ReturnsAsync(accessToken);
            _merchantServiceMock.Setup(o => o.UpsertMerchantDetailsAsync(It.IsAny<MerchantDetails>(), It.IsAny<TokenInfo>())).ThrowsAsync(new Exception());

            //act & assert
            await Assert.ThrowsAsync<Exception>(async () => await _merchantController.CreateMerchantAsync(merchantDetails));
        }

        [Fact]
        public async Task UpsertMerchantDetailsAsync_Returns_False_Throws_InvalidOperationException()
        {
            //arrange
            var _fixture = new Fixture();
            var accessToken = _fixture.Create<MockBankClient.AccessToken>();
            var merchantDetails = _fixture.Create<MerchantDetails>();
            _validatorMock.Setup(x => x.ValidateAsync(It.IsAny<MerchantDetails>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult { Errors = new List<ValidationFailure>() });
            _mockBankAuthMock.Setup(o => o.GenerateClientCredentialsAsync(It.IsAny<MerchantDetails>())).ReturnsAsync(accessToken);
            _merchantServiceMock.Setup(o => o.UpsertMerchantDetailsAsync(It.IsAny<MerchantDetails>(), It.IsAny<TokenInfo>())).ReturnsAsync(false);

            //act & assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await _merchantController.CreateMerchantAsync(merchantDetails));
        }
    }
}
