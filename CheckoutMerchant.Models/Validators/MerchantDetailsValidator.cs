using CheckoutMerchant.Models.Merchant;
using FluentValidation;

namespace CheckoutMerchant.Models.Validators
{
    public class MerchantDetailsValidator : AbstractValidator<MerchantDetails>
    {
        public MerchantDetailsValidator()
        {
            RuleFor(x => x.Id).NotNull().NotEmpty();
            RuleFor(x => x.Key).NotNull().NotEmpty();
        }
    }
}
