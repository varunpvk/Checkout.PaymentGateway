namespace CheckoutMerchant.Models
{
    public class ProcessPaymentResponse
    {
        public string PaymentId { get; init; }

        public string ConsentId { get; init; }

        public string Status { get; init; }

        public string CreatedDateTime { get; init; }

        public string StatusUpdateDateTime { get; init; }

        public Initiation Initiation { get; init; }

        public CreditorAccount CreditorAccount { get; init; }

        public Link PaymentStatusLink { get; init; }
    }
}
