using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Tekwill.Library.Infrastructure.Handlers.Authentication
{
    class AppHeaderAuthHandler : AuthenticationHandler<AppHeaderAuthOptions>
    {

        public AppHeaderAuthHandler(IOptionsMonitor<AppHeaderAuthOptions> options, ILoggerFactory logger, UrlEncoder encoder) :base(options, logger, encoder)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue("x-app-name", out var value))
            {
                return Task.FromResult(AuthenticateResult.NoResult());
            }

            var name = value.FirstOrDefault();
            if (string.IsNullOrEmpty(name))
            {
                return Task.FromResult(AuthenticateResult.Fail("Missing x-app-name"));
            }
            if (Options.AllowedNames.Contains(name, StringComparer.Ordinal))
            {
                return Task.FromResult(AuthenticateResult.Fail("Invalid x-app-name"));
            }

            var claims = new Claim[]
            {
                new Claim(ClaimTypes.Name, name),
                new Claim("app", name)
            };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
