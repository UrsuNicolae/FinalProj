using Tekwill.Library.Application.Common;
using Tekwill.Library.Domain.Entities;

namespace Tekwill.Library.Application.Interfaces
{
    public interface IBookRepository
    {

        Task<PaginatedList<Book>> GetBooks(int page, int pageSize, CancellationToken ct = default);
        Task<Book> GetBook(int id, CancellationToken ct = default);

        Task<Book> CreateBook(Book book, CancellationToken ct = default);
        Task UpdateBook(Book book, CancellationToken ct = default);
        Task DeleteBook(int id, CancellationToken ct = default);

        Task<Book> FindByText(string title, CancellationToken ct = default);

        Task<List<Book>> GetLastBooks(CancellationToken ct = default);
    }
}
