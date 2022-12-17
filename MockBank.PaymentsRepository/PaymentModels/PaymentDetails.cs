using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentsRepository.PaymentModels
{
    public record PaymentRequestDetails
    {
        public string PaymentId { get; init; }
        public string ConsentId { get; init; }
        public string AccessToken { get; init; }
        public string PaymentStatus { get; init; }
        public string PaymentDetails { get; init; }
        public DateTime CreatedDate { get; init; }
        public DateTime LastUpdatedDate { get; init; }
    }
}
