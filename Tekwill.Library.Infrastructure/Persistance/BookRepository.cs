using Tekwill.Library.Application.Common;
using Tekwill.Library.Application.Interfaces;
using Tekwill.Library.Domain.Entities;

namespace Tekwill.Library.Infrastructure.Persistance
{
    public class BookRepository : IBookRepository
    {
        public Task CreateBook(Book book, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task DeleteBook(int id, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<Book> GetBook(int id, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<PaginatedList<Book>> GetBooks(int page, int pageSize, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task UpdateBook(Book book, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
    }
}
