namespace CheckoutMerchant.Models
{
    public record Initiation
    {
        public string InstructionIdentifier { get; init; }

        public InstructedAmount InstructedAmount { get; init; }
    }
}
