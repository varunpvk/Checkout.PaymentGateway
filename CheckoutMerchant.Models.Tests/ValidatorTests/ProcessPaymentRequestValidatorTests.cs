using AutoFixture;
using CheckoutMerchant.Models.Validators;
using FluentValidation.TestHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckoutMerchant.Models.Tests.ValidatorTests
{
    public class ProcessPaymentRequestValidatorTests
    {
        private ProcessPaymentRequestValidator validator;

        public ProcessPaymentRequestValidatorTests()
        {
            validator = new ProcessPaymentRequestValidator();
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("12341234")]
        [InlineData("12341234123412341234")]
        public async Task Should_Have_ValidationError_For_CardNumber(string value)
        {
            //arrange
            var fixture = new Fixture();
            var model = fixture.Create<ProcessPaymentRequest>() with
            {
                CreditorAccount = fixture.Create<CreditorAccount>() with
                {
                    CardNumber = value
                }
            };

            //act
            var result = await validator.TestValidateAsync(model);

            //assert
            result.ShouldHaveValidationErrorFor(model => model.CreditorAccount.CardNumber);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(null)]
        [InlineData(-1)]
        [InlineData(20)]
        [InlineData(99)]
        [InlineData(1000)]
        public async Task Should_Have_ValidationError_For_Cvv(int value)
        {
            //arrange
            var fixture = new Fixture();
            var model = fixture.Create<ProcessPaymentRequest>() with
            {
                CreditorAccount = fixture.Create<CreditorAccount>() with
                {
                    Cvv = value
                }
            };

            //act
            var result = await validator.TestValidateAsync(model);

            //assert
            result.ShouldHaveValidationErrorFor(model => model.CreditorAccount.Cvv);
        }

        [Theory]
        [InlineData("")]
        [InlineData("02")]
        [InlineData("02/")]
        [InlineData("/21")]
        [InlineData("01/22")]
        [InlineData("12/21")]
        public async Task Should_Have_ValidationError_For_ExpiryDate(string value)
        {
            //arrange
            var fixture = new Fixture();
            var model = fixture.Create<ProcessPaymentRequest>() with
            {
                CreditorAccount = fixture.Create<CreditorAccount>() with
                {
                    ExpiryDate = value
                }
            };

            //act
            var result = await validator.TestValidateAsync(model);

            //assert
            result.ShouldHaveValidationErrorFor(model => model.CreditorAccount.ExpiryDate);
        }

        [Theory]
        [InlineData(0)]
        public async Task Should_Have_ValidationError_For_Amount(double value)
        {
            //arrange
            var fixture = new Fixture();
            var model = fixture.Create<ProcessPaymentRequest>() with
            {
                Initiation = fixture.Create<Initiation>() with
                {
                    InstructedAmount = fixture.Create<InstructedAmount>() with
                    {
                        Amount = value
                    }
                }
            };

            //act
            var result = await validator.TestValidateAsync(model);

            //assert
            result.ShouldHaveValidationErrorFor(model => model.Initiation.InstructedAmount.Amount);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("ABCD")]
        [InlineData("AB")]
        public async Task Should_Have_ValidationError_For_Currency(string value)
        {
            //arrange
            var fixture = new Fixture();
            var model = fixture.Create<ProcessPaymentRequest>() with
            {
                Initiation = fixture.Create<Initiation>() with
                {
                    InstructedAmount = fixture.Create<InstructedAmount>() with
                    {
                        Currency = value
                    }
                }
            };

            //act
            var result = await validator.TestValidateAsync(model);

            //assert
            result.ShouldHaveValidationErrorFor(model => model.Initiation.InstructedAmount.Currency);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task Should_Have_ValidationError_For_InstructionIdentifier(string value)
        {
            //arrange
            var fixture = new Fixture();
            var model = fixture.Create<ProcessPaymentRequest>() with
            {
                Initiation = fixture.Create<Initiation>() with
                {
                    InstructionIdentifier = value
                }
            };

            //act
            var result = await validator.TestValidateAsync(model);

            //assert
            result.ShouldHaveValidationErrorFor(model => model.Initiation.InstructionIdentifier);
        }
    }
}
