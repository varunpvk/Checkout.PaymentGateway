using FluentValidation;

namespace CheckoutMerchant.Models.Validators
{
    public class ProcessPaymentRequestValidator : AbstractValidator<ProcessPaymentRequest>
    {
        public ProcessPaymentRequestValidator()
        {
            RuleFor(x => x.CreditorAccount).NotNull();
            RuleFor(x => x.CreditorAccount.CardNumber)
                .NotEmpty()
                .NotNull()
                .Length(16);
            RuleFor(x => x.CreditorAccount.Cvv)
                .NotEmpty()
                .NotNull()
                .Must(o => o >= 100 && o <= 999);
            RuleFor(x => x.CreditorAccount.ExpiryDate)
                .NotNull()
                .NotEmpty()
                .Must(o => ValidateExpireyDate(o));
            RuleFor(x => x.Initiation).NotNull();
            RuleFor(x => x.Initiation.InstructedAmount).NotNull();
            RuleFor(x => x.Initiation.InstructedAmount.Amount)
                .NotEmpty()
                .NotNull()
                .Must(o => o > 0);
            RuleFor(x => x.Initiation.InstructedAmount.Currency)
                .NotEmpty()
                .NotNull()
                .Must(o => ISO._4217.CurrencyCodesResolver.Codes.Any(p => p.Code == o));
            RuleFor(x => x.Initiation.InstructionIdentifier)
                .NotNull()
                .NotEmpty();
        }

        private bool ValidateExpireyDate(string expiryDate)
        {
            string[] date = expiryDate.Split("/");
            bool isExpiryDateValid = false;
            var currYear = DateTime.UtcNow.Year % 2000;

            if (date.Length == 2 && date[0].Length == 2 && date[1].Length == 2)
            {
                if (int.TryParse(date[0], out var month))
                {
                    isExpiryDateValid = month > 0 && month <= 12;

                    if (isExpiryDateValid && int.TryParse(date[1], out var year))
                    {
                        isExpiryDateValid = year > 0 && year <= 99;

                        if (isExpiryDateValid)
                        {
                            if (year > currYear)
                            {
                                isExpiryDateValid = true;
                            }
                            else

                            if (year == currYear)
                            {
                                if (month >= DateTime.UtcNow.Month)
                                {
                                    isExpiryDateValid = true;
                                }
                                else
                                {
                                    isExpiryDateValid = false;
                                }
                            }
                            else
                            {
                                isExpiryDateValid = false;
                            }
                        }
                    }
                }
            }

            return isExpiryDateValid;
        }
    }
}
