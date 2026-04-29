using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Tekwill.Library.Application.DTOs.Categories;
using Tekwill.Library.Application.DTOs.Gens;
using Tekwill.Library.Application.Interfaces;
using Tekwill.Library.Application.Profiles;
using Tekwill.Library.Infrastructure.Configuration;
using Tekwill.Library.Infrastructure.Data;
using Tekwill.Library.Infrastructure.Implementations;
using Tekwill.Library.Infrastructure.Persistance;

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

        public static IServiceCollection ConfigureRepositories(this IServiceCollection services)
        {
            services.AddScoped<IGenRepository, GenRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IBookRepository, BookRepository>();
            services.AddScoped<IAuthorRepository, AuthorRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            return services;
        }

        public static IServiceCollection ConfigureAutoMapper(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(GenProfile).Assembly);
            return services;
        }

        public static IServiceCollection ConfigureJwtAuth(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
             .AddJwtBearer(options =>
             {
                 options.TokenValidationParameters = new TokenValidationParameters
                 {
                     ValidateIssuer = true,
                     ValidateAudience = true,
                     ValidateLifetime = true,
                     ValidateIssuerSigningKey = true,
                     ValidIssuer = configuration["JwtSettings:Issuer"],
                     ValidAudience = configuration["JwtSettings:Audience"],
                     IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:SecretKey"]))
                 };
             });
            return services;
        }

        public static IServiceCollection ConfigureFluentValidators(this IServiceCollection services)
        {
            services.AddFluentValidation(fv => fv.RegisterValidatorsFromAssembly(typeof(CreateGenDtoValidator).Assembly));
            return services;
        }

        public static IServiceCollection ConfigureAuthServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IGoogleService, GoogleService>();
            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
            services.Configure<GoogleConfiguration>(configuration.GetSection(GoogleConfiguration.SectionName));
            services.AddHttpClient<IGoogleService, GoogleService>(client =>
            {
                client.BaseAddress = new Uri(configuration[$"{GoogleConfiguration.SectionName}:TokenUrl"]);
            });
            return services;
        }
    }
}
