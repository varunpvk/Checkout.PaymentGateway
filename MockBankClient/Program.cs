using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Threading.Tasks;
using Grpc.Core;
using Grpc.Net.Client;
using JWT.Algorithms;
using JWT.Builder;
using MockBankClient;
//using Newtonsoft.Json;

// The port number must match the port of the gRPC server.
using var channel = GrpcChannel.ForAddress("https://localhost:7240");
var client = new MockBank.MockBankClient(channel);

//var authClient = new MockBankAuth.MockBankAuthClient(channel);

//var headers = new Metadata();
//headers.Add("Authorization", "Bearer token");


//var token = JwtBuilder.Create()
//                    .WithAlgorithm(new RS256Algorithm(RSA.Create("RS256Algorithm")))
//                    .AddClaim("exp", DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeSeconds())
//                    .AddClaim("claim1", "varun")
//                    .Encode();
//Console.WriteLine(token);

//var replyAuth = await authClient.GenerateAccessTokenAsync(new ClientCredentials
//{
//    IdempotencyKey = Guid.NewGuid().ToString("d"),
//    InteractionId = Guid.NewGuid().ToString("d"),
//    JwsSignature = "private_key"
//}, headers: headers, deadline: DateTime.UtcNow.AddMinutes(60));

while (Console.ReadKey().Key != ConsoleKey.UpArrow)
{
    var paymentInitiationRequest = new InitiatePaymentRequest
    {
        PaymentId = Guid.NewGuid().ToString("d"),
        CreditorAccount = new CreditorAccount
        {
            CardNumber = "1234567891234567",
            Cvv = 123,
            ExpiryDate = "10/23"
        },
        Initiation = new Initiation
        {
            InstructionIdentifier = Guid.NewGuid().ToString("d"),
            InstructedAmount = new InstructedAmount
            {
                Amount = 65.75,
                Currency = "GBP"
            }
        }
    };
    Console.WriteLine("Initiate Payment");
    var paymentInititationResponse = await client.InitiatePaymentAsync(paymentInitiationRequest);
    var jsonResponse = JsonSerializer.Serialize(paymentInititationResponse, new JsonSerializerOptions
    {
        WriteIndented = true,
    });
    Console.WriteLine(jsonResponse);

    var paymentRequest = new PaymentRequest
    {
        ConsentId = paymentInititationResponse.ConsentId,
        CreditorAccount = paymentInititationResponse.CreditorAccount,
        Initiation = paymentInititationResponse.Initiation,
        PaymentId = paymentInititationResponse.PaymentId
    };
    Console.WriteLine("Process Payment");
    var processPaymentResponse = await client.ProcessPaymentAsync(paymentRequest);
    var jsonProcessPaymentResponse = JsonSerializer.Serialize(processPaymentResponse, new JsonSerializerOptions
    {
        WriteIndented = true
    });
    Console.WriteLine(jsonProcessPaymentResponse);

    Console.WriteLine("get Payment");
    var getPaymentResponse = await client.GetPaymentAsync(new PaymentIdRequest { PaymentId = paymentInititationResponse.PaymentId });
    var jsonGetPaymentRespone = JsonSerializer.Serialize(getPaymentResponse, new JsonSerializerOptions { WriteIndented = true });
    Console.WriteLine(jsonGetPaymentRespone);
    Console.WriteLine("-----------------------------------------------------------------------------------------------------");
}

Console.WriteLine("Press any key to exit...");
Console.ReadKey();
