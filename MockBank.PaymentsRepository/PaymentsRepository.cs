using Dapper;
using Microsoft.Extensions.Configuration;
using PaymentsRepository.PaymentModels;
using System.Data.SqlClient;
using System.Text.Json;

namespace PaymentsRepository
{
    public class PaymentsRepository : IPaymentsRepository
    {
        private readonly IConfiguration _config;
        
        public PaymentsRepository(IConfiguration config)
        {
            _config = config;
        }

        public async Task<bool> CreatePaymentAsync(PaymentRequestDetails paymentDetails)
        {
            string paymentsDBConnStr = _config.GetConnectionString("PaymentsDB");
            string sql = "INSERT INTO PAYMENTDETAILS " +
                "(PaymentId, ConsentId, AccessToken, PaymentStatus, PaymentDetails, CreatedDateTime, LastUpdatedDateTime) " +
                "VALUES " +
                "(@paymentId, @consentId, @accessToken, @paymentStatus, @paymentDetails, @createdDateTime, @lastUpdatedDateTime)";

            var parameters = new DynamicParameters();
            parameters.Add("PaymentId", paymentDetails.PaymentId);
            parameters.Add("ConsentId", paymentDetails.ConsentId);
            parameters.Add("AccessToken", paymentDetails.AccessToken);
            parameters.Add("PaymentStatus", paymentDetails.PaymentStatus);
            parameters.Add("PaymentDetails", paymentDetails.PaymentDetails);
            parameters.Add("CreatedDateTime", paymentDetails.CreatedDate);
            parameters.Add("LastUpdatedDateTime", paymentDetails.LastUpdatedDate);

            using(var connection = new SqlConnection(paymentsDBConnStr))
            {
                var r = await connection.ExecuteAsync(sql, parameters);

                if (r == 1)
                    return true;
                else
                    return false;
            }

            throw new InvalidOperationException("Unable to execute the query");
        }

        public async Task<PaymentRequestDetails> GetPaymentAsync(string paymentId)
        {
            string paymentsDBConnStr = _config.GetConnectionString("PaymentsDB");
            string sql = $"SELECT * FROM PAYMENTDETAILS WHERE PaymentId = '{paymentId}'";

            using (var connection = new SqlConnection(paymentsDBConnStr))
            {
                var result = await connection.QueryAsync<PaymentRequestDetails>(sql);
                if (result.Count() == 1)
                {
                    return result.First();
                }
            }

            throw new InvalidOperationException("Unable to execute the query");
        }

        public async Task<bool> UpdatePaymentAsync(PaymentRequestDetails paymentDetails)
        {
            string paymentsDBConnStr = _config.GetConnectionString("PaymentsDB");
            string sql = "UPDATE PAYMENTDETAILS " +
                "SET ConsentId = @consentId, AccessToken = @accessToken, PaymentStatus = @paymentStatus, PaymentDetails = @paymentDetails, LastUpdatedDateTime = @LastUpdatedDateTime " +
                "WHERE " +
                "PaymentId = @paymentId";

            var parameters = new DynamicParameters();
            
            parameters.Add("ConsentId", paymentDetails.ConsentId);
            parameters.Add("AccessToken", paymentDetails.AccessToken);
            parameters.Add("PaymentStatus", paymentDetails.PaymentStatus);
            parameters.Add("PaymentDetails", paymentDetails.PaymentDetails);
            parameters.Add("LastUpdatedDateTime", paymentDetails.LastUpdatedDate, System.Data.DbType.DateTime);
            parameters.Add("PaymentId", paymentDetails.PaymentId);

            using (var connection = new SqlConnection(paymentsDBConnStr))
            {
                var r = await connection.ExecuteAsync(sql, parameters);

                if (r == 1)
                    return true;
                else
                    return false;
            }

            throw new InvalidOperationException("Unable to execute the query");
        }
    }
}
