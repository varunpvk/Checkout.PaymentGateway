using AutoFixture;
using CheckoutMerchant.Models.Merchant;
using CheckoutMerchant.Models.Validators;
using FluentValidation.TestHelper;

namespace CheckoutMerchant.Models.Tests.ValidatorTests
{
    public class MerchantDetailsValidatorTests
    {
        private MerchantDetailsValidator validator;

        public MerchantDetailsValidatorTests()
        {
            validator = new MerchantDetailsValidator();
        }

        [Fact]
        public async Task Should_Have_Error_When_Id_IsNull()
        {
            //Arrange
            var fixture = new Fixture();
            var model = fixture.Create<MerchantDetails>() with
            {
                Id = null
            };

            //Act
            var result = await validator.TestValidateAsync(model);

            //Assert
            result.ShouldHaveValidationErrorFor(model => model.Id);
        }

        [Fact]
        public async Task Should_Have_Error_When_Id_IsEmpty()
        {
            //Arrange
            var fixture = new Fixture();
            var model = fixture.Create<MerchantDetails>() with
            {
                Id = ""
            };

            //Act
            var result = await validator.TestValidateAsync(model);

            //Assert
            result.ShouldHaveValidationErrorFor(model => model.Id);
        }

        [Fact]
        public async Task Should_Have_Error_When_Key_IsNull()
        {
            //Arrange
            var fixture = new Fixture();
            var model = fixture.Create<MerchantDetails>() with
            {
                Key = null
            };

            //Act
            var result = await validator.TestValidateAsync(model);

            //Assert
            result.ShouldHaveValidationErrorFor(model => model.Key);
        }

        [Fact]
        public async Task Should_Have_Error_When_Key_IsEmpty()
        {
            //Arrange
            var fixture = new Fixture();
            var model = fixture.Create<MerchantDetails>() with
            {
                Key = ""
            };

            //Act
            var result = await validator.TestValidateAsync(model);

            //Assert
            result.ShouldHaveValidationErrorFor(model => model.Key);
        }
    }
}
