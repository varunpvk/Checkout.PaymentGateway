using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckoutMerchant.Models.Validators
{
    public class MerchantPaymentDetailsValidator : AbstractValidator<MerchantPaymentDetails>
    {
        public MerchantPaymentDetailsValidator()
        {
            RuleFor(x => x.PaymentId)
                .NotNull()
                .NotEmpty();
            RuleFor(x => x.InstructionIdentifier)
                .NotNull()
                .NotEmpty();
        }
    }
}
