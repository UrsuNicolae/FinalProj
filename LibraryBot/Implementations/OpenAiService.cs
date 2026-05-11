using LibraryBot.Dtos;
using LibraryBot.Interfaces;
using System.Text;
using System.Text.Json;
using Tekwill.Library.Application.DTOs.Authors;
using Tekwill.Library.Application.DTOs.Books;
using Tekwill.Library.Application.DTOs.Categories;
using Tekwill.Library.Domain.Entities;

namespace LibraryBot.Implementations
{
    public class OpenAiService : IOpenAiService
    {
        private readonly HttpClient httpClient;
        private readonly string model;
        private readonly JsonSerializerOptions options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };

        public OpenAiService(HttpClient httpClient, IConfiguration configuration)
        {
            this.httpClient = httpClient;
            model = configuration["OpenAi:Model"];
        }
        public async Task<string> GetBookRecommandations(BookDto book, AuthorDto authorDto, CategoryDto categoryDto, int count, CancellationToken ct)
        {
            var prompt = $"Based on the following book:" +
                book.ToString() +
                authorDto.ToString() +
                categoryDto.ToString() +
                $"Give {count} books similar to this.";
            return await SendOpenAiRequest(prompt, ct);

        }

        private async Task<string> SendOpenAiRequest(string prompt, CancellationToken ct)
        {
            var request = new OpenAiRequest
            {
                Model = model,
                Messages = new[]
                {
                    new OpenAiMessage
                    {
                        Role = "system",
                        Content = SystemPrompt
                    },
                    new OpenAiMessage
                    {
                        Role = "user",
                        Content = prompt
                    }
                },
                Temperature = 0.7
            };

            var json = JsonSerializer.Serialize(request, options);

            using var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync("v1/chat/completions", content, ct);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Open ai returned status code: {response.StatusCode}");
            }
            var responseJson = await response.Content.ReadAsStringAsync(ct);
            var openAiResponse = JsonSerializer.Deserialize<OpenAiReponse>(responseJson, options);
            return openAiResponse?.Choices.FirstOrDefault()?.Message.Content;
        }

        private static string SystemPrompt = "You are an expert book recommendation assistant.\r\n\r\nYour task is to recommend books based on a user-provided book title, author, and category/genre. Use the given book as the main reference point for tone, themes, writing style, subject matter, audience, and reading experience.\r\n\r\nWhen the user provides:\r\n- Book title\r\n- Author\r\n- Category or genre\r\n\r\nYou should return personalized book recommendations that are similar, complementary, or likely to appeal to someone who enjoyed that book.\r\n\r\nGuidelines:\r\n1. Recommend 5 to 10 books.\r\n2. Prioritize books that match the given category or genre.\r\n3. Consider similarities in:\r\n   - Themes\r\n   - Writing style\r\n   - Mood or tone\r\n   - Plot structure\r\n   - Subject matter\r\n   - Target audience\r\n   - Level of complexity\r\n4. Include a mix of popular, well-reviewed, and slightly lesser-known titles when appropriate.\r\n5. Do not recommend the exact same book the user provided.\r\n6. If the input book is part of a series, you may recommend the next book in the series, but clearly say so.\r\n7. If the title, author, or category is unclear, make a reasonable assumption and mention it briefly.\r\n8. Avoid spoilers.\r\n9. Keep explanations concise and useful.\r\n\r\nFor each recommendation, provide:\r\n- Book title\r\n- Author\r\n- Category/genre\r\n- A short reason why it matches the user’s book\r\n- A “best for readers who want...” note\r\n\r\nResponse format:\r\n\r\nBased on: [Book Title] by [Author]  \r\nCategory: [Category]\r\n\r\nRecommended books:\r\n\r\n1. [Book Title] by [Author]  \r\n   Genre: [Genre]  \r\n   Why it fits: [Brief explanation]  \r\n   Best for readers who want: [Specific appeal]\r\n\r\n2. [Book Title] by [Author]  \r\n   Genre: [Genre]  \r\n   Why it fits: [Brief explanation]  \r\n   Best for readers who want: [Specific appeal]\r\n\r\nEnd with a brief summary explaining the overall recommendation direction.";
    }
}
