using FluentValidation.Internal;
using LibraryBot.Helper;
using LibraryBot.Interfaces;
using System.Text;
using Tekwill.Library.Application.Common;
using Tekwill.Library.Application.DTOs.Authors;
using Tekwill.Library.Application.DTOs.Books;
using Tekwill.Library.Application.DTOs.Categories;
using Tekwill.Library.Application.Interfaces;
using Tekwill.Library.Domain.Entities;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

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
            if (update.Type == UpdateType.CallbackQuery)
            {
                await HandleCallback(bot, update.CallbackQuery, ct);
                return;
            }

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
                            replyMarkup: GetMainMenuKeyboard(),
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
                                var kimiService = scope.ServiceProvider.GetRequiredService<IKimiService>();
                                var result = await kimiService.GetBookRecommandations(book, author, category, 5, ct);
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

        private async Task HandleCallback(ITelegramBotClient bot, CallbackQuery callbackQuery, CancellationToken ct)
        {
            var data = callbackQuery.Data;
            var chatId = callbackQuery.Message.Chat.Id;

            if (data == "main_menu")
            {
                await bot.EditMessageText(
                    chatId: chatId,
                    messageId: callbackQuery.Message.MessageId,
                    text: LibraryBot.Helper.MessageFormatter.GetWelcomeMessage(),
                    parseMode: ParseMode.MarkdownV2,
                    replyMarkup: GetMainMenuKeyboard(),
                    cancellationToken: ct);
                await bot.AnswerCallbackQuery(callbackQuery.Id, cancellationToken: ct);
                return;
            }
            if (data.StartsWith("books_") || data.StartsWith("authors_") || data.StartsWith("categories_"))
            {
                using var scope = scopeFactory.CreateAsyncScope();
                var parts = data.Split("_");
                var type = parts[0];
                var pageSize = int.Parse(parts[1]);
                var pageIndex = int.Parse(parts[2]);
                var libApiClient = scope.ServiceProvider.GetRequiredService<ILibraryApiClient>();
                switch (type)
                {
                    case "books":
                        var books = await libApiClient.GetPaginatedBooks(pageSize, pageIndex, ct);
                        await bot.EditMessageText(
                            chatId: chatId,
                            messageId: callbackQuery.Message.MessageId,
                            text: FormatPaginatedBooks(books),
                            parseMode: ParseMode.MarkdownV2,
                            replyMarkup: GetPaginatedKeyboard("books", pageSize, pageIndex, books.TotalPages),
                            cancellationToken: ct
                            );
                        break;
                    case "authors":
                        var authors = await libApiClient.GetPaginatedAuthors(pageSize, pageIndex, ct);
                        await bot.EditMessageText(
                            chatId: chatId,
                            messageId: callbackQuery.Message.MessageId,
                            text: FormatPaginatedAuthors(authors),
                            parseMode: ParseMode.MarkdownV2,
                            replyMarkup: GetPaginatedKeyboard("authors", pageSize, pageIndex, authors.TotalPages),
                            cancellationToken: ct
                            );
                        break;
                    case "categories":
                        var categories = await libApiClient.GetPaginatedCategories(pageSize, pageIndex, ct);
                        await bot.EditMessageText(
                            chatId: chatId,
                            messageId: callbackQuery.Message.MessageId,
                            text: FormatPaginatedCategories(categories),
                            parseMode: ParseMode.MarkdownV2,
                            replyMarkup: GetPaginatedKeyboard("categories", pageSize, pageIndex, categories.TotalPages),
                            cancellationToken: ct
                            );
                        break;
                }
            }
            await bot.AnswerCallbackQuery(callbackQuery.Id, cancellationToken: ct);
        }

        private InlineKeyboardMarkup GetMainMenuKeyboard()
        {
            return new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("📖 Browse Books", "books_10_1"),
                    InlineKeyboardButton.WithCallbackData("✍️ Browse Authors", "authors_10_1")
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("📂 Browse Categories", "categories_10_1")
                }
            });
        }

        private InlineKeyboardMarkup GetPaginatedKeyboard(string type, int pageSize, int currentPage, int totalPages)
        {
            var buttons = new List<InlineKeyboardButton[]>();
            var navigationButtons = new List<InlineKeyboardButton>();
            if (currentPage > 1)
            {
                navigationButtons.Add(InlineKeyboardButton.WithCallbackData("⬅️ Previous", $"{type}_{pageSize}_{currentPage - 1}"));
            }
            navigationButtons.Add(InlineKeyboardButton.WithCallbackData($"📄 {currentPage}/{totalPages}", "invalid-command"));
            if (currentPage < totalPages)
            {
                navigationButtons.Add(InlineKeyboardButton.WithCallbackData("Next ➡️", $"{type}_{pageSize}_{currentPage + 1}"));
            }
            buttons.Add(navigationButtons.ToArray());
            buttons.Add(new[] { InlineKeyboardButton.WithCallbackData("🔙 Back to Menu", "main_menu") });
            return new InlineKeyboardMarkup(buttons);
        }

        private string FormatPaginatedBooks(PaginatedList<BookDto> paginatedBooks)
        {
            var text = new StringBuilder("📚 *Books List*\n\n");
            foreach (var book in paginatedBooks.Items)
            {
                text.Append($"📖    *{LibraryBot.Helper.MessageFormatter.EscapeMarkdownV2(book.Title ?? "N/A")}*\n");
                text.Append($"ID:   *{LibraryBot.Helper.MessageFormatter.EscapeMarkdownV2(book.Id)}*\n");
                text.Append($"ISBN: *{LibraryBot.Helper.MessageFormatter.EscapeMarkdownV2(book.ISBN)}*\n");
            }
            text.Append($"\n Page {LibraryBot.Helper.MessageFormatter.EscapeMarkdownV2(paginatedBooks.PageIndex)} of {LibraryBot.Helper.MessageFormatter.EscapeMarkdownV2(paginatedBooks.TotalPages)}");
            return text.ToString();
        }

        private string FormatPaginatedAuthors(PaginatedList<AuthorDto> paginatedAuthors)
        {
            var text = new StringBuilder("✍️ *Authors List*\n\n");
            foreach (var author in paginatedAuthors.Items)
            {
                text.Append($"🧑‍🏫    *{LibraryBot.Helper.MessageFormatter.EscapeMarkdownV2(author.FirstName ?? "N/A")} {LibraryBot.Helper.MessageFormatter.EscapeMarkdownV2(author.LastName ?? "")}*\n");
                text.Append($"ID:   *{LibraryBot.Helper.MessageFormatter.EscapeMarkdownV2(author.Id)}*\n");
                text.Append($"Books: *{LibraryBot.Helper.MessageFormatter.EscapeMarkdownV2(author.Books?.Count() ?? 0)}*\n");
            }
            text.Append($"\n Page {LibraryBot.Helper.MessageFormatter.EscapeMarkdownV2(paginatedAuthors.PageIndex)} of {LibraryBot.Helper.MessageFormatter.EscapeMarkdownV2(paginatedAuthors.TotalPages)}");
            return text.ToString();
        }

        private string FormatPaginatedCategories(PaginatedList<CategoryDto> paginatedCategories)
        {
            var text = new StringBuilder("📂 *Categories List*\n\n");
            foreach (var category in paginatedCategories.Items)
            {
                text.Append($"📂    *{LibraryBot.Helper.MessageFormatter.EscapeMarkdownV2(category.Name ?? "N/A")}*\n");
                text.Append($"ID:   *{LibraryBot.Helper.MessageFormatter.EscapeMarkdownV2(category.Id)}*\n");
            }
            text.Append($"\n Page {LibraryBot.Helper.MessageFormatter.EscapeMarkdownV2(paginatedCategories.PageIndex)} of {LibraryBot.Helper.MessageFormatter.EscapeMarkdownV2(paginatedCategories.TotalPages)}");
            return text.ToString();
        }
    }
}
