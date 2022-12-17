using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;
using Jose;
using Jose.keys;
using System.Security.Cryptography.X509Certificates;

namespace MockBankAPI.Services
{
    public class MockBankAuthService : MockBankAuth.MockBankAuthBase
    {
        private readonly ILogger<MockBankAuthService> _logger;

        public MockBankAuthService(ILogger<MockBankAuthService> logger)
        {
            _logger = logger;
        }

        public override Task<AccessToken> GenerateAccessToken(ClientCredentials clientCredentials, ServerCallContext context)
        {
            var payload = new Dictionary<string, object>
            {
                ["issuer"] =  clientCredentials.ClientId,
                ["key"] = clientCredentials.ClientSecret
            };
            var secretKey = new byte[] { 164, 60, 194, 0, 161, 189, 41, 38, 130, 89, 141, 164, 45, 170, 159, 209, 69, 137, 243, 216, 191, 131, 47, 250, 32, 107, 231, 117, 37, 158, 225, 234 };
            var token = Jose.JWT.Encode(payload, secretKey, JwsAlgorithm.HS256);

            var accessToken = new AccessToken
            {
                Token = token,
                Scope = "payments",
                Status = "approved",
                TokenType = "bearer",
                ExpiresIn = DateTime.UtcNow.AddHours(1).Ticks
            };

            return Task.FromResult(accessToken);
        }

        public override Task<AccessToken> RefreshToken(ClientCredentials request, ServerCallContext context)
        {
            throw new NotImplementedException();
        }
    }
}
