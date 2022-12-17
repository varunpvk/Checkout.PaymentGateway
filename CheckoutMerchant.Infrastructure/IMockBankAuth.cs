using CheckoutMerchant.Models.Merchant;
using MockBankClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckoutMerchant.Infrastructure
{
    public interface IMockBankAuth
    {
        Task<AccessToken> GenerateClientCredentialsAsync(MerchantDetails merchantDetails);
    }
}
