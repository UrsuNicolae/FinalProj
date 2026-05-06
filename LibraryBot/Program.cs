using LibraryBot.Implementations;
using LibraryBot.Interfaces;
using Tekwill.Library.Infrastructure.Extensions;
using Telegram.Bot;

namespace LibraryBot
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);
            builder.Services.AddHostedService<Worker>();
            builder.Services.ConfigureEfCore(builder.Configuration);
            builder.Services.ConfigureBotRepositories();
            builder.Services.AddSingleton<ITelegramBotClient>(s =>
            {
                var token = builder.Configuration["BotApiToken"];
                return new TelegramBotClient(token);
            });

            builder.Services.AddHttpClient(Constants.LibraryApiClient, (_, c) =>
            {
                c.BaseAddress = new Uri(builder.Configuration["LibraryApi:BaseAddress"]);
                c.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            });

            builder.Services.AddScoped<ILibraryApiClient, LibraryApiClient>();

            var host = builder.Build();
            host.Run();
        }
    }
}