using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Moq;
using Tekwill.Library.Application.Interfaces;
using Tekwill.Library.Infrastructure.Configuration;
using Tekwill.Library.Infrastructure.Implementations;

namespace Tekwill.Library.Tests.Infrastracture
{
    public class GoogleServiceTests
    {
        [Theory]
        [InlineData("http://localhost", "testId", "http://localhost/redirect")]
        public void GetRedirectLinkShouldCreateBasedOnConfigurationParams(string baseUrl, string clientId, string redirectUrl)
        {
            var configurationMoq = new Mock<IOptions<GoogleConfiguration>>();
            var googleConfiguration = new GoogleConfiguration
            {
                BaseUrl = baseUrl,
                ClientId = clientId,
                RedirectPath = redirectUrl
            };

            configurationMoq.Setup(s => s.Value).Returns(googleConfiguration);
            IGoogleService service = new GoogleService(configurationMoq.Object, new HttpClient());

            var link = service.GetRedirectLink();
            Assert.True(link.StartsWith(baseUrl));
            Assert.True(link.Contains($"client_id={clientId}"));
            Assert.True(link.Contains($"redirect_uri={Uri.EscapeDataString(redirectUrl)}"));
        }
    }
}
