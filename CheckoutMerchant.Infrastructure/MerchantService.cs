using CheckoutMerchant.Models;
using CheckoutMerchant.Models.Merchant;
using CheckoutMerchant.Models.Validators;
using Dapper;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Text.Json;

namespace CheckoutMerchant.Infrastructure
{
    public class MerchantService : IMerchantService
    {
        private readonly IConfiguration _config;

        public MerchantService(IConfiguration config)
        {
            _config = config;
        }

        public async Task<TokenInfo> GetAccessTokenAsync(string merchantId)
        {
            string ckoMerchantDBConn = _config.GetConnectionString("CheckoutMerchantDB");
            string sql = $"Select AccessToken, ExpiresIn from MerchantDetails where MerchantId = '{merchantId}'";

            using (var conn = new SqlConnection(ckoMerchantDBConn))
            {
                var result = await conn.QueryAsync<TokenInfo>(sql);

                if(result.Count() == 1)
                {
                    return result.First();
                }

                throw new ValidationException($"Must exist 1 record for {merchantId}");
            }
        }

        public async Task<bool> SaveMerchantDetailsAsync(MerchantDetails merchantDetails, TokenInfo tokenInfo)
        {
            string ckoMerchantDBConn = _config.GetConnectionString("CheckoutMerchantDB");
            string sql = "Insert into MerchantDetails " +
                "(MerchantId, MerchantDetails, AccessToken, ExpiresIn) " +
                "values " +
                "(@merchantId, @merchantDetails, @accessToken, @expiresIn)";

            var jsonMerchantDetails = JsonSerializer.Serialize(merchantDetails);

            var parameters = new DynamicParameters();
            parameters.Add("MerchantId", merchantDetails.Id);
            parameters.Add("MerchantDetails", jsonMerchantDetails);
            parameters.Add("AccessToken", tokenInfo.AccessToken);
            parameters.Add("ExpiresIn", tokenInfo.ExpiresIn);

            using (var conn = new SqlConnection(ckoMerchantDBConn))
            {
                var result = await conn.ExecuteAsync(sql, parameters);

                if (result == 1)
                    return true;

                return false;
            }

            throw new InvalidOperationException("Unable to execute the query");
        }

        private async Task<bool> UpdateMerchantDetailsAsync(MerchantDetails merchantDetails, TokenInfo tokenInfo)
        {
            string ckoMerchantDBConn = _config.GetConnectionString("CheckoutMerchantDB");
            var sql = $"UPDATE MerchantDetails SET " +
                $"MerchantDetails = @merchantDetails, AccessToken = @accessToken, ExpiresIn = @expiresIn " +
                $"WHERE " +
                $"MerchantId = @merchantId";

            var jsonMerchantDetails = JsonSerializer.Serialize(merchantDetails);

            var parameters = new DynamicParameters();
            parameters.Add("MerchantDetails", jsonMerchantDetails);
            parameters.Add("AccessToken", tokenInfo.AccessToken);
            parameters.Add("ExpiresIn", tokenInfo.ExpiresIn);
            parameters.Add("MerchantId", merchantDetails.Id);

            using(var conn = new SqlConnection(ckoMerchantDBConn))
            {
                var result = await conn.ExecuteAsync(sql, parameters);

                if (result == 1)
                    return true;

                return false;
            }
        }

        public async Task<bool> UpsertMerchantDetailsAsync(MerchantDetails merchantDetails, TokenInfo tokenInfo)
        {
            string ckoMerchantDBConn = _config.GetConnectionString("CheckoutMerchantDB");
            string sql = $"SELECT * FROM MerchantDetails WHERE MerchantId = '{merchantDetails.Id}'";

            using (var conn = new SqlConnection(ckoMerchantDBConn))
            {
                var result = await conn.QueryAsync<string>(sql);

                if(result.Count() == 1)
                {
                    bool isMerchantUpdated = await UpdateMerchantDetailsAsync(merchantDetails, tokenInfo);

                    return isMerchantUpdated;

                }
                else
                {
                    var isMerchantCreated = await SaveMerchantDetailsAsync(merchantDetails, tokenInfo);

                    return isMerchantCreated;
                }
                
            }
        }
    }
}
