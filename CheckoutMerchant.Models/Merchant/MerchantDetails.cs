using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckoutMerchant.Models.Merchant
{
    public record MerchantDetails
    {
        public string Id { get; init; }
        public string Key { get; init; }
    }
}
