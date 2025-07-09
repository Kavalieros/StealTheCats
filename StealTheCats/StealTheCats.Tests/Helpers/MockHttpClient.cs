using Moq;
using Moq.Protected;
using StealTheCats.Helpers;
using System.Net;

namespace StealTheCats.Tests.Helpers
{
    public static class MockHttpClient
    {
        public static HttpClient Create(string responseContent, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

            handlerMock
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>()
               )
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = statusCode,
                   Content = new StringContent(responseContent),
               })
               .Verifiable();

            var client = new HttpClient(handlerMock.Object)
            {
                BaseAddress = CatsUrlHelper.BaseUri
            };

            return client;
        }
    }
}
