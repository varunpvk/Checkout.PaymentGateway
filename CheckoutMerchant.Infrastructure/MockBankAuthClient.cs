using CheckoutMerchant.Models.Merchant;
using Grpc.Net.Client;
using MockBankClient;

namespace CheckoutMerchant.Infrastructure
{
    public class MockBankAuthClient : IMockBankAuth
    {
        private readonly GrpcChannel _channel;
        private readonly MockBankAuth.MockBankAuthClient _client;

        public MockBankAuthClient()
        {
            _channel = GrpcChannel.ForAddress("http://MockBank.API");
            _client = new MockBankAuth.MockBankAuthClient(_channel);
        }

        public async Task<AccessToken> GenerateClientCredentialsAsync(MerchantDetails merchantDetails)
        {
            var accessToken = await _client.GenerateAccessTokenAsync(new ClientCredentials
            {
                ClientId = merchantDetails.Id,
                ClientSecret = merchantDetails.Key,
                GrantType = "client_credentials",
                Scope = "payments"
            });

            return accessToken;
        }
    }
}
