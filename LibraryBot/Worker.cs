using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace LibraryBot
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly ITelegramBotClient bot;

        public Worker(ILogger<Worker> logger, ITelegramBotClient bot)
        {
            _logger = logger;
            this.bot = bot;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //while (!stoppingToken.IsCancellationRequested)
            //{
            //    if (_logger.IsEnabled(LogLevel.Information))
            //    {
            //        _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            //    }
            //    await Task.Delay(1000, stoppingToken);
            //}
            var receiver = new ReceiverOptions { AllowedUpdates = Array.Empty<UpdateType>() };
            bot.StartReceiving(HandleUpdate, HandleError, receiver, stoppingToken);
        }

        private async Task HandleError(ITelegramBotClient client, Exception exception, HandleErrorSource source, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        private async Task HandleUpdate(ITelegramBotClient bot, Update update, CancellationToken ct)
        {
            Console.WriteLine("Message received");
            await bot.SendMessage(
                chatId: update.Message.Chat.Id,
                text: "test from local bot"
                );
        }
    }
}
