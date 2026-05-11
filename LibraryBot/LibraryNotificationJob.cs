using Quartz;
using System.Text;
using Tekwill.Library.Application.Interfaces;
using Tekwill.Library.Domain.Entities;
using Telegram.Bot;

namespace LibraryBot
{
    [DisallowConcurrentExecution]
    public class LibraryNotificationJob : IJob
    {
        private readonly ILogger<LibraryNotificationJob> logger;
        private readonly ITelegramBotClient bot;
        private readonly IServiceScopeFactory scopeFactory;

        public LibraryNotificationJob(ILogger<LibraryNotificationJob> logger, ITelegramBotClient bot, IServiceScopeFactory scopeFactory)
        {
            this.logger = logger;
            this.bot = bot;
            this.scopeFactory = scopeFactory;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            logger.LogInformation($"{nameof(LibraryNotificationJob)} has started!");
            try
            {
                using var scope = scopeFactory.CreateAsyncScope();
                var chatRepo = scope.ServiceProvider.GetRequiredService<IChatRepository>();
                var bookRepository = scope.ServiceProvider.GetRequiredService<IBookRepository>();
                var chats = await chatRepo.GetAllChatsForNewBookNotification(default);
                var books = await bookRepository.GetLastBooks();
                if (books.Any())
                {
                    foreach(var chat in chats)
                    {
                        await bot.SendMessage(
                            chatId: chat.ChatId,
                            text: FormatPaginatedBooks(books),
                            parseMode: Telegram.Bot.Types.Enums.ParseMode.MarkdownV2);
                    }
                }
            }
            catch(Exception e)
            {
                logger.LogError(e, e.Message);
            }
            finally
            {
                logger.LogInformation($"{nameof(LibraryNotificationJob)} finished!");
            }
        }

        private string FormatPaginatedBooks(List<Book> books)
        {
            var text = new StringBuilder($"📚 *Books List* \n\n");
            for(int index = 0; index < books.Count(); index++)
            {
                text.Append($"Nr {index + 1} \n");
                text.Append($"📖 *{books[index].Title??"N/A"}* \n");
                text.Append($"ID: *{books[index].Id}* \n");
                text.Append($"ISBN: *{books[index].ISBN}* \n");
            }
            return text.ToString();
        }
    }
}
