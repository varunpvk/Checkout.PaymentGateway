using AutoFixture;
using Grpc.Core;
using Grpc.Core.Testing;
using MockBankAPI.Services;
using Moq;
using PaymentsRepository;
using PaymentsRepository.PaymentModels;

namespace MockBankAPI.Tests
{
    public class MockBankServiceTests
    {
        private readonly Mock<IPaymentsRepository> _paymentsRepository;
        private readonly MockBankService _mockBankService;

        public MockBankServiceTests()
        {
            _paymentsRepository = new Mock<IPaymentsRepository>();
            _mockBankService = new MockBankService(_paymentsRepository.Object);
        }

        [Fact(Skip = "Still underdevelopment")]
        public async Task TestsAsync()
        {
            //arrange
            var _fixture = new Fixture();
            var _initiatePaymentRequest = _fixture.Create<InitiatePaymentRequest>();
            //var _httpContext = new DefaultHttpContext();
            //_httpContext.Request.Headers["Authorization"] = "test-header";

            var _context = TestServerCallContext.Create(method: nameof(MockBankServiceTests)
                                            , host: "localhost"
                                            , deadline: DateTime.Now.AddMinutes(30)
                                            , requestHeaders: new Metadata()
                                            {
                                                {"Authorization", "test-header" }
                                            }
                                            , cancellationToken: CancellationToken.None
                                            , peer: "10.0.0.25:5001"
                                            , authContext: null
                                            , contextPropagationToken: null
                                            , writeHeadersFunc: (metadata) => Task.CompletedTask
                                            , writeOptionsGetter: () => new WriteOptions()
                                            , writeOptionsSetter: (writeOptions) => { }
                                            );

            _paymentsRepository
                .Setup(o => o.CreatePaymentAsync(It.IsAny<PaymentRequestDetails>()))
                .ReturnsAsync(true);

            //act
            var result = await _mockBankService.InitiatePayment(_initiatePaymentRequest, _context);

            //assert
        }
    }
}
