namespace CheckoutMerchant.Models
{
    public record InstructedAmount
    {
        public double Amount { get; init; }

        public string Currency { get; init; }
    }
}
