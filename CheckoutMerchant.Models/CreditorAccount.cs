namespace CheckoutMerchant.Models
{
    public record CreditorAccount
    {
        public string CardNumber { get; init; }

        public string ExpiryDate { get; init; }
        
        public int Cvv { get; init; }
    }
}
