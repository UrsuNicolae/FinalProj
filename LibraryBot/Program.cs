using LibraryBot.Implementations;
using LibraryBot.Interfaces;
using Quartz;
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
                c.DefaultRequestHeaders.Add("x-app-name", builder.Configuration["LibraryApi:AppName"]);
            });
            builder.Services.AddHttpClient<IKimiService, KimiService>(client =>
            {
                client.BaseAddress = new Uri(builder.Configuration["Kimi:BaseAddress"]);
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + builder.Configuration["Kimi:ApiKey"]);
            });
            builder.Services.AddScoped<ILibraryApiClient, LibraryApiClient>();
            builder.Services.AddQuartz(q =>
            {
                var notificationJobKey = new JobKey(nameof(LibraryNotificationJob));
                q.AddJob<LibraryNotificationJob>(o => o.WithIdentity(notificationJobKey));
                q.AddTrigger(o => o.ForJob(notificationJobKey)
                .WithIdentity($"{notificationJobKey}_trigger")
                .WithSimpleSchedule(x => x.WithIntervalInSeconds(30).RepeatForever()
                .WithMisfireHandlingInstructionNextWithExistingCount()));
            });

            builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

            var host = builder.Build();
            host.Run();
        }
    }
}