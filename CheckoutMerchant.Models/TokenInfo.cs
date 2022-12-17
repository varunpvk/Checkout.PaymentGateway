using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckoutMerchant.Models
{
    public record TokenInfo
    {
        public string AccessToken { get; init; }
        public long ExpiresIn { get; init; }
    }
}
