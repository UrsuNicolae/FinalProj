using Tekwill.Library.Application.DTOs.Authors;
using Tekwill.Library.Application.DTOs.Books;
using Tekwill.Library.Application.DTOs.Categories;

namespace LibraryBot.Interfaces
{
    public interface IOpenAiService
    {
        Task<string> GetBookRecommandations(BookDto book, AuthorDto authorDto, CategoryDto categoryDto, int count, CancellationToken ct);
    }
}
