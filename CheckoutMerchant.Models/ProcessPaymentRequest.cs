namespace CheckoutMerchant.Models
{
    public record ProcessPaymentRequest
    {
        public Initiation Initiation { get; init; }

        public CreditorAccount CreditorAccount { get; init; }
    }
}