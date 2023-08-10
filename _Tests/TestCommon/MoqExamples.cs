using Moq;
using Moq.Protected;

namespace MoqExamples
{
    [TestClass]
    public class scratchPad
    {
        public interface IRandomNumberGenerator
        {
			int Seed { get; set; }
            int Next();
            int Next(int minValue, int maxValue);
        }

        [TestMethod]
        public void constant_return_method()
        {
            var moq1 = new Mock<IRandomNumberGenerator>();
            moq1.Setup(a => a.Next()).Returns(3);
            moq1.Object.Next().Should().Be(3);
        }

        [TestMethod]
        public void constant_return_get()
        {
            var seed = -12345;
            var moq1 = new Mock<IRandomNumberGenerator>();
            moq1.SetupGet(a => a.Seed).Returns(seed);
            moq1.Object.Seed.Should().Be(seed);
        }

        [TestMethod]
        public void conditional_return()
        {
            var moq = new Mock<IRandomNumberGenerator>();
            moq.Setup(z => z.Next(995, 1005)).Returns(1001);
            var rng = moq.Object;

            var undefined = rng.Next(1, 2);
            undefined.Should().Be(0);

            var _1001 = rng.Next(995, 1005);
            _1001.Should().Be(1001);
        }

        [TestMethod]
        public void return_sequentially()
        {
            var _mockClient = new Mock<IRandomNumberGenerator>();
			var seed1 = 98765;
			var seed2 = -12345;
            _mockClient.SetupSequence(m => m.Seed)
                // 1st call to Now returns this value
                .Returns(seed1)
                // 2nd call returns this value
                .Returns(seed2);
            var obj = _mockClient.Object;
            obj.Seed.Should().Be(seed1);
            obj.Seed.Should().Be(seed2);
        }

        [TestMethod]
        public async Task complex_sequence()
        {
            // ARRANGE mock
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handlerMock
                .Protected()
                .Setup("Dispose", ItExpr.IsAny<bool>())
                .Verifiable();

            handlerMock
                .Protected()
                .SetupSequence<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                // 1st handler call: throws
                .ThrowsAsync(new TimeoutException())
                //  2nd handler call: passes
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = System.Net.HttpStatusCode.ServiceUnavailable,
                    Content = new StringContent("foo bar")
                })
                //  3rd handler call: throws
                .ThrowsAsync(new ArgumentException())
                ;

            var client = new HttpClient(handlerMock.Object);
            async Task<HttpResponseMessage> callClient()
                => await client.SendAsync(
                    new HttpRequestMessage(
                        System.Net.Http.HttpMethod.Get,
                        new Uri("http://test.com")
                        )
                    );

            // ACT and ASSERT

            // 1st call
            await Assert.ThrowsExceptionAsync<TimeoutException>(() => callClient());

            // 2nd call
            var response = await callClient();
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.ServiceUnavailable);
            (await response.Content.ReadAsStringAsync()).Should().Be("foo bar");

            // 3rd call
            await Assert.ThrowsExceptionAsync<ArgumentException>(() => callClient());
        }
    }
}
