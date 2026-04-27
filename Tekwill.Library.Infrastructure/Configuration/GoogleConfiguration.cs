namespace Tekwill.Library.Infrastructure.Configuration
{
    public class GoogleConfiguration
    {
        public static string SectionName => nameof(GoogleConfiguration);
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string BaseUrl { get; set; }
        public string RedirectPath { get; set; }
    }
}
