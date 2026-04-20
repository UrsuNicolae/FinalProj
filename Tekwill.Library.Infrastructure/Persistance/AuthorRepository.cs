using Microsoft.EntityFrameworkCore;
using SQLitePCL;
using Tekwill.Library.Application.Common;
using Tekwill.Library.Application.Interfaces;
using Tekwill.Library.Domain.Entities;
using Tekwill.Library.Infrastructure.Data;

namespace Tekwill.Library.Infrastructure.Persistance
{
    public class AuthorRepository : IAuthorRepository
    {
        private readonly LibraryContext context;

        public AuthorRepository(LibraryContext context)
        {
            this.context = context;
        }
        public async Task CreateAuthor(Author author, CancellationToken ct = default)
        {
            await context.Authors.AddAsync(author, ct);
            await context.SaveChangesAsync(ct);
        }

        public async Task DeleteAuthor(int id, CancellationToken ct = default)
        {
            var author = await context.Authors.FirstOrDefaultAsync(a => a.Id == id, ct);
            if (author == null)
                throw new KeyNotFoundException($"Invalid id:{id}");

            context.Authors.Remove(author);
            await context.SaveChangesAsync(ct);
        }

        public async Task<Author> GetAuthor(int id, CancellationToken ct = default)
        {
            var author = await context.Authors.FirstOrDefaultAsync(a => a.Id == id, ct);
            if (author == null)
                throw new KeyNotFoundException($"Invalid id:{id}");

            return author;
        }

        public async Task<PaginatedList<Author>> GetAuthors(int page, int pageSize, CancellationToken ct = default)
        {
            var count = await context.Authors.CountAsync(ct);
            var authors = await context.Authors.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
            return new PaginatedList<Author>(authors, pageSize, (int)Math.Ceiling((decimal)count / pageSize));
        }

        public async Task UpdateAuthor(Author author, CancellationToken ct = default)
        {
            var authorFromDb = await context.Authors.FirstOrDefaultAsync(a => a.Id == author.Id, ct);
            if (authorFromDb == null)
                throw new KeyNotFoundException($"Invalid id:{author.Id}");

            authorFromDb.FirstName = author.FirstName;
            authorFromDb.LastName = author.LastName;
            authorFromDb.BirthDate = author.BirthDate;
            authorFromDb.Site = author.Site ;
            context.Authors.Update(authorFromDb);
            await context.SaveChangesAsync(ct);
        }
    }
}
