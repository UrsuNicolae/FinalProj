using Tekwill.Library.Application.Common;
using Tekwill.Library.Application.DTOs.Authors;
using Tekwill.Library.Application.DTOs.Books;
using Tekwill.Library.Application.DTOs.Categories;

namespace LibraryBot.Interfaces
{
    public interface ILibraryApiClient
    {
        Task<AuthorDto> GetAuthorById(int id, CancellationToken ct);
        Task<BookDto> GetBooksById(int id, CancellationToken ct);
        Task<BookDto> GetBooksByQuery(string query, CancellationToken ct);
        Task<CategoryDto> GetCategoriesById(int id, CancellationToken ct);

        Task<PaginatedList<AuthorDto>> GetPaginatedAuthors(int pageSize, int pageIndex, CancellationToken ct);
        Task<PaginatedList<BookDto>> GetPaginatedBooks(int pageSize, int pageIndex, CancellationToken ct);
        Task<PaginatedList<CategoryDto>> GetPaginatedCategories(int pageSize, int pageIndex, CancellationToken ct);


    }
}
