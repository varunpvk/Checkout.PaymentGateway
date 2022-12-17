namespace CheckoutMerchant.Models
{
    public record MerchantPaymentDetails
    {
        public string PaymentId { get; init; }
        public string InstructionIdentifier { get; init; }
    }
}
