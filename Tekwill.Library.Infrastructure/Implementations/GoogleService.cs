using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Text.Json;
using Tekwill.Library.Application.Interfaces;
using Tekwill.Library.Infrastructure.Configuration;

namespace Tekwill.Library.Infrastructure.Implementations
{
    public class GoogleService : IGoogleService
    {
        private readonly GoogleConfiguration configuration;
        private readonly HttpClient httpClient;

        public GoogleService(IOptions<GoogleConfiguration> configuration, HttpClient httpClient)
        {
            this.configuration = configuration.Value;
            this.httpClient = httpClient;
        }

        public async Task<string> GetIdToken(string code, CancellationToken ct = default)
        {
            var tokenRequest = new HttpRequestMessage(HttpMethod.Post, "/token");
            tokenRequest.Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                {"code", code },
                {"client_id", configuration.ClientId },
                {"client_secret", configuration.ClientSecret },
                {"redirect_uri", configuration.RedirectPath },
                {"grant_type", "authorization_code" }
            });

            var tokenResponse = await httpClient.SendAsync(tokenRequest, ct);
            var tokenResponseContent = await tokenResponse.Content.ReadAsStringAsync(ct);
            var tokenData = JsonSerializer.Deserialize<JsonElement>(tokenResponseContent);
            var idToken = tokenData.GetProperty("id_token").ToString();
            return idToken;
        }

        public string GetRedirectLink()
        {
            var scope = "openid email profile";
            var url = $"{configuration.BaseUrl}" +
                $"client_id={configuration.ClientId}" +
                $"&redirect_uri={Uri.EscapeDataString(configuration.RedirectPath)}" +
                $"&response_type=code" +
                $"&scope={Uri.EscapeDataString(scope)}";
            return url;
        }
    }
}
