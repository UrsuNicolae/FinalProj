using Microsoft.EntityFrameworkCore;
using Tekwill.Library.Application.Common;
using Tekwill.Library.Application.Interfaces;
using Tekwill.Library.Domain.Entities;
using Tekwill.Library.Infrastructure.Data;

namespace Tekwill.Library.Infrastructure.Persistance
{
    public class BookRepository : IBookRepository
    {
        private readonly LibraryContext context;

        public BookRepository(LibraryContext context)
        {
            this.context = context;
        }

        public async Task CreateBook(Book book, CancellationToken ct = default)
        {
            await context.Books.AddAsync(book, ct);
            await context.SaveChangesAsync(ct);
        }

        public async Task DeleteBook(int id, CancellationToken ct = default)
        {
            var book = await context.Books.FirstOrDefaultAsync(a => a.Id == id, ct);
            if (book == null)
                throw new KeyNotFoundException($"Invalid id:{id}");

            context.Books.Remove(book);
            await context.SaveChangesAsync(ct);
        }

        public async Task<Book> GetBook(int id, CancellationToken ct = default)
        {
            var book = await context.Books.FirstOrDefaultAsync(a => a.Id == id, ct);
            if (book == null)
                throw new KeyNotFoundException($"Invalid id:{id}");
            return book;
        }

        public async Task<PaginatedList<Book>> GetBooks(int page, int pageSize, CancellationToken ct = default)
        {
            var count = await context.Books.CountAsync(ct);
            var books = await context.Books.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
            return new PaginatedList<Book>(books, pageSize, (int)Math.Ceiling((decimal)count / pageSize));
        }

        public async Task UpdateBook(Book book, CancellationToken ct = default)
        {
            var bookFromDb = await context.Books.FirstOrDefaultAsync(a => a.Id == book.Id, ct);
            if (bookFromDb == null)
                throw new KeyNotFoundException($"Invalid id:{book.Id}");

            bookFromDb.ISBN = book.ISBN;
            bookFromDb.Title = book.Title;
            bookFromDb.AuthorId = book.AuthorId;
            bookFromDb.CategoryId = book.CategoryId;
            context.Books.Update(bookFromDb);
            await context.SaveChangesAsync(ct);
        }
    }
}
