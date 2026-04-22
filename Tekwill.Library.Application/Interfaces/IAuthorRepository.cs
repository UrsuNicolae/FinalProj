using Tekwill.Library.Application.Common;
using Tekwill.Library.Domain.Entities;

namespace Tekwill.Library.Application.Interfaces
{
    public interface IAuthorRepository
    {
        Task<PaginatedList<Author>> GetAuthors(int page, int pageSize, CancellationToken ct = default);
        Task<Author> GetAuthor(int id, CancellationToken ct = default);

        Task<Author> CreateAuthor(Author author, CancellationToken ct = default);
        Task UpdateAuthor (Author author, CancellationToken ct = default);
        Task DeleteAuthor(int id, CancellationToken ct = default);
    }
}
