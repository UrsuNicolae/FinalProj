using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tekwill.Library.Infrastructure.Data;

namespace Tekwill.Library.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureEfCore(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<LibraryContext>(options =>
            {
                options.UseSqlite(configuration.GetConnectionString("Default"));
            });
            return services;
        }
    }
}
