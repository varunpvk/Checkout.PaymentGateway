using CheckoutMerchant.Models;
using CheckoutMerchant.Models.Merchant;

namespace CheckoutMerchant.Infrastructure
{
    public interface IMerchantService
    {
        Task<bool> SaveMerchantDetailsAsync(MerchantDetails merchantDetails, TokenInfo tokenInfo);
        Task<bool> UpsertMerchantDetailsAsync(MerchantDetails merchantDetails, TokenInfo tokenInfo);
        Task<TokenInfo> GetAccessTokenAsync(string merchantId);
    }
}
