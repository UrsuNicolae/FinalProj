using FluentValidation.Internal;
using LibraryBot.Helper;
using LibraryBot.Interfaces;
using Tekwill.Library.Application.Interfaces;
using Tekwill.Library.Domain.Entities;
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
        private readonly IServiceScopeFactory scopeFactory;

        public Worker(ILogger<Worker> logger, ITelegramBotClient bot, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            this.bot = bot;
            this.scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var receiver = new ReceiverOptions { AllowedUpdates = Array.Empty<UpdateType>() };
            bot.StartReceiving(HandleUpdate, HandleError, receiver, stoppingToken);
        }

        private Task HandleError(ITelegramBotClient client, Exception exception, HandleErrorSource source, CancellationToken token)
        {
            _logger.LogError(exception, exception.Message);
            return Task.CompletedTask;
        }

        private async Task HandleUpdate(ITelegramBotClient bot, Update update, CancellationToken ct)
        {
            var chatId = update.Message.Chat.Id;
            var text = update.Message.Text;

            using var scope = scopeFactory.CreateAsyncScope();
            switch (text)
            {
                case string s when s.Equals("/start"):
                    {
                        var chatRepository = scope.ServiceProvider.GetRequiredService<IChatRepository>();
                        var chatFromDb = await chatRepository.GetChat(chatId, ct);
                        if (chatFromDb == null)
                        {
                            var chatToCreate = new Tekwill.Library.Domain.Entities.Chat
                            {
                                Id = update.Message.Chat.Id,
                                UserName = update.Message.Chat.Username ?? "",
                                LastName = update.Message.Chat.LastName ?? "",
                                FirstName = update.Message.Chat.FirstName ?? "",
                                IsForm = update.Message.Chat.IsForum,
                                Type = update.Message.Chat.Type.ToString()
                            };
                            await chatRepository.CreateChat(chatToCreate, ct);
                        }
                        await bot.SendMessage(
                            chatId: chatId,
                            text: LibraryBot.Helper.MessageFormatter.GetWelcomeMessage(),
                            parseMode: ParseMode.MarkdownV2,
                            //replyMarkup: 
                            cancellationToken: ct
                            );
                        break;
                    }
                case string s when s.StartsWith("/book:"):
                    {
                        var libApiClient = scope.ServiceProvider.GetRequiredService<ILibraryApiClient>();
                        var splits = s.Split(":", StringSplitOptions.RemoveEmptyEntries);
                        if (splits.Count() == 2 &&
                            int.TryParse(splits[1], out var id))
                        {
                            var book = await libApiClient.GetBooksById(id, ct);
                            if (book == null)
                            {
                                await bot.SendMessage(
                                    chatId: chatId,
                                    text: "Upss book not found!",
                                    cancellationToken: ct);
                            }
                            else
                            {
                                await bot.SendMessage(
                                    chatId: chatId,
                                    text: book.ToString(),
                                    cancellationToken: ct);
                            }
                        }
                        else
                        {
                            await bot.SendMessage(
                                chatId: chatId,
                                text: "Invalid command",
                                cancellationToken: ct);
                        }
                        break;
                    }
                case string s when s.StartsWith("/search:"):
                    {
                        var libApiClient = scope.ServiceProvider.GetRequiredService<ILibraryApiClient>();
                        var splits = s.Split(":", StringSplitOptions.RemoveEmptyEntries);
                        if (splits.Count() == 2)
                        {
                            var book = await libApiClient.GetBooksByQuery(splits[1], ct);
                            if (book == null)
                            {
                                await bot.SendMessage(
                                    chatId: chatId,
                                    text: "Upss book not found!",
                                    cancellationToken: ct);
                            }
                            else
                            {
                                await bot.SendMessage(
                                    chatId: chatId,
                                    text: book.ToString(),
                                    cancellationToken: ct);
                            }
                        }
                        else
                        {
                            await bot.SendMessage(
                                chatId: chatId,
                                text: "Invalid command",
                                cancellationToken: ct);
                        }
                        break;
                    }
                case string s when s.StartsWith("/books:"):
                    {
                        var libApiClient = scope.ServiceProvider.GetRequiredService<ILibraryApiClient>();
                        var splits = s.Split(":", StringSplitOptions.RemoveEmptyEntries);
                        if (splits.Count() == 3 &&
                            int.TryParse(splits[1], out var pageSize) &&
                            int.TryParse(splits[2], out var pageIndex))
                        {
                            var paginatedBooks = await libApiClient.GetPaginatedBooks(pageSize, pageIndex, ct);
                            if (paginatedBooks == null || paginatedBooks.Items.Count() == 0)
                            {
                                await bot.SendMessage(
                                    chatId: chatId,
                                    text: LibraryBot.Helper.MessageFormatter.GetNoDataMessage(),
                                    cancellationToken: ct
                                    );
                            }
                            else
                            {
                                await bot.SendMessage(
                                    chatId: chatId,
                                    text: paginatedBooks.ToString(),
                                    cancellationToken: ct);
                            }
                        }
                        else
                        {
                            await bot.SendMessage(
                                chatId: chatId,
                                text: "Invalid command",
                                cancellationToken: ct);
                        }
                        break;
                    }
                case string s when s.StartsWith("/author:"):
                    {
                        var libApiClient = scope.ServiceProvider.GetRequiredService<ILibraryApiClient>();
                        var splits = s.Split(":", StringSplitOptions.RemoveEmptyEntries);
                        if (splits.Count() == 2 &&
                            int.TryParse(splits[1], out var id))
                        {
                            var author = await libApiClient.GetAuthorById(id, ct);
                            if (author == null)
                            {
                                await bot.SendMessage(
                                    chatId: chatId,
                                    text: "Upss author not found!",
                                    cancellationToken: ct);
                            }
                            else
                            {
                                await bot.SendMessage(
                                    chatId: chatId,
                                    text: author.ToString(),
                                    cancellationToken: ct);
                            }
                        }
                        else
                        {
                            await bot.SendMessage(
                                chatId: chatId,
                                text: "Invalid command",
                                cancellationToken: ct);
                        }
                        break;
                    }
                case string s when s.StartsWith("/authors:"):
                    {
                        var libApiClient = scope.ServiceProvider.GetRequiredService<ILibraryApiClient>();
                        var splits = s.Split(":", StringSplitOptions.RemoveEmptyEntries);
                        if (splits.Count() == 3 &&
                            int.TryParse(splits[1], out var pageSize) &&
                            int.TryParse(splits[2], out var pageIndex))
                        {
                            var paginatedAuthors = await libApiClient.GetPaginatedAuthors(pageSize, pageIndex, ct);
                            if (paginatedAuthors == null || paginatedAuthors.Items.Count() == 0)
                            {
                                await bot.SendMessage(
                                    chatId: chatId,
                                    text: LibraryBot.Helper.MessageFormatter.GetNoDataMessage(),
                                    cancellationToken: ct
                                    );
                            }
                            else
                            {
                                await bot.SendMessage(
                                    chatId: chatId,
                                    text: paginatedAuthors.ToString(),
                                    cancellationToken: ct);
                            }
                        }
                        else
                        {
                            await bot.SendMessage(
                                chatId: chatId,
                                text: "Invalid command",
                                cancellationToken: ct);
                        }
                        break;
                    }
                case string s when s.StartsWith("/category:"):
                    {
                        var libApiClient = scope.ServiceProvider.GetRequiredService<ILibraryApiClient>();
                        var splits = s.Split(":", StringSplitOptions.RemoveEmptyEntries);
                        if (splits.Count() == 2 &&
                            int.TryParse(splits[1], out var id))
                        {
                            var category = await libApiClient.GetCategoriesById(id, ct);
                            if (category == null)
                            {
                                await bot.SendMessage(
                                    chatId: chatId,
                                    text: "Upss category not found!",
                                    cancellationToken: ct);
                            }
                            else
                            {
                                await bot.SendMessage(
                                    chatId: chatId,
                                    text: category.ToString(),
                                    cancellationToken: ct);
                            }
                        }
                        break;
                    }
                case string s when s.StartsWith("/categories:"):
                    {
                        var libApiClient = scope.ServiceProvider.GetRequiredService<ILibraryApiClient>();
                        var splits = s.Split(":", StringSplitOptions.RemoveEmptyEntries);
                        if (splits.Count() == 3 &&
                            int.TryParse(splits[1], out var pageSize) &&
                            int.TryParse(splits[2], out var pageIndex))
                        {
                            var paginatedCategories = await libApiClient.GetPaginatedCategories(pageSize, pageIndex, ct);
                            if (paginatedCategories == null || paginatedCategories.Items.Count() == 0)
                            {
                                await bot.SendMessage(
                                    chatId: chatId,
                                    text: LibraryBot.Helper.MessageFormatter.GetNoDataMessage(),
                                    cancellationToken: ct
                                    );
                            }
                            else
                            {
                                await bot.SendMessage(
                                    chatId: chatId,
                                    text: paginatedCategories.ToString(),
                                    cancellationToken: ct);
                            }
                        }
                        else
                        {
                            await bot.SendMessage(
                                chatId: chatId,
                                text: "Invalid command",
                                cancellationToken: ct);
                        }
                        break;
                    }
                case string s when s.StartsWith("/recomendation:"):
                    {
                        var libClient = scope.ServiceProvider.GetRequiredService<ILibraryApiClient>();
                        var splits = s.Split(":", StringSplitOptions.RemoveEmptyEntries);
                        if (splits.Count() == 2 && int.TryParse(splits[1], out var id))
                        {
                            var book = await libClient.GetBooksById(id, ct);
                            if (book == null)
                            {
                                await bot.SendMessage(chatId: chatId, text: "Upss book not found!", cancellationToken: ct);
                            }
                            else
                            {
                                var category = await libClient.GetCategoriesById(book.CategoryId, ct);
                                var author = await libClient.GetAuthorById(book.AuthorId, ct);
                                var openAiService = scope.ServiceProvider.GetRequiredService<IOpenAiService>();
                                var result = await openAiService.GetBookRecommandations(book, author, category, 5, ct);
                                await bot.SendMessage(chatId: chatId, text: result, cancellationToken: ct);
                            }

                        }
                        else
                        {
                            await bot.SendMessage(
                                chatId: chatId,
                                text: "Invalid command",
                                cancellationToken: ct);
                        }
                        break;
                    }
                case string s when s.Equals("/subscribe"):
                    {
                        var chatRepo = scope.ServiceProvider.GetRequiredService<IChatRepository>();
                        await chatRepo.CreateChatNotification(chatId, Tekwill.Library.Domain.Enums.NotificationType.NewBookNotification, ct);
                        await bot.SendMessage(
                                    chatId: chatId,
                                    text: "Subscribed successfully!",
                                    cancellationToken: ct);
                        break;
                    }
                case string s when s.Equals("/unsubscribe"):
                    {
                        var chatRepo = scope.ServiceProvider.GetRequiredService<IChatRepository>();
                        await chatRepo.DeleteChatNotification(chatId, Tekwill.Library.Domain.Enums.NotificationType.NewBookNotification, ct);
                        await bot.SendMessage(
                                    chatId: chatId,
                                    text: "Unsubscribed successfully!",
                                    cancellationToken: ct);
                        break;
                    }
                default:
                    await bot.SendMessage(
                                chatId: chatId,
                                text: "Invalid command",
                                cancellationToken: ct);
                    break;
            }
        }
    }
}
