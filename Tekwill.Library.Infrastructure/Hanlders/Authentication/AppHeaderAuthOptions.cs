using Microsoft.AspNetCore.Authentication;

namespace Tekwill.Library.Infrastructure.Handlers.Authentication
{
    public class AppHeaderAuthOptions : AuthenticationSchemeOptions
    {
        public string[] AllowedNames { get; set; }
    }
}
