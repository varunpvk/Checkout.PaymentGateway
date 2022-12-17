using AutoFixture;
using CheckoutMerchant.Models.Validators;
using FluentValidation.TestHelper;

namespace CheckoutMerchant.Models.Tests.ValidatorTests
{
    public class MerchantPaymentDetailsValidatorTests
    {
        private MerchantPaymentDetailsValidator validator;

        public MerchantPaymentDetailsValidatorTests()
        {
            validator = new MerchantPaymentDetailsValidator();
        }

        [Fact]
        public async Task Should_Have_Error_When_PaymentId_IsNull()
        {
            //arrange
            var fixture = new Fixture();
            var model = fixture.Create<MerchantPaymentDetails>() with
            {
                PaymentId = null
            };

            //act
            var result = await validator.TestValidateAsync(model);

            //assert
            result.ShouldHaveValidationErrorFor(model => model.PaymentId);
        }

        [Fact]
        public async Task Should_Have_Error_When_PaymentId_IsEmpty()
        {
            //arrange
            var fixture = new Fixture();
            var model = fixture.Create<MerchantPaymentDetails>() with
            {
                PaymentId = ""
            };

            //act
            var result = await validator.TestValidateAsync(model);

            //assert
            result.ShouldHaveValidationErrorFor(model => model.PaymentId);
        }

        [Fact]
        public async Task Should_Have_Error_When_InstructionIdentifier_IsNull()
        {
            //arrange
            var fixture = new Fixture();
            var model = fixture.Create<MerchantPaymentDetails>() with
            {
                InstructionIdentifier = null
            };

            //act
            var result = await validator.TestValidateAsync(model);

            //assert
            result.ShouldHaveValidationErrorFor(model => model.InstructionIdentifier);
        }

        [Fact]
        public async Task Should_Have_Error_When_InstructionIdentifier_IsEmpty()
        {
            //arrange
            var fixture = new Fixture();
            var model = fixture.Create<MerchantPaymentDetails>() with
            {
                InstructionIdentifier = ""
            };

            //act
            var result = await validator.TestValidateAsync(model);

            //assert
            result.ShouldHaveValidationErrorFor(model => model.InstructionIdentifier);
        }
    }
}
