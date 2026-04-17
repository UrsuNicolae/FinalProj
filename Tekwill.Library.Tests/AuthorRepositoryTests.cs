using Microsoft.EntityFrameworkCore;
using Tekwill.Library.Application.Interfaces;
using Tekwill.Library.Domain.Entities;
using Tekwill.Library.Infrastructure.Data;
using Tekwill.Library.Infrastructure.Persistance;

namespace Tekwill.Library.Tests
{
    public class AuthorRepositoryTests
    {
        [Fact]
        public async Task CreateAuthorShourSaveInDb()
        {
            var options = new DbContextOptionsBuilder<LibraryContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            using var libContext = new LibraryContext(options);
            IAuthorRepository repo = new AuthorRepository(libContext);
            var author = new Author
            {
                FirstName = "TestName",
                LastName = "TestLastName",
                BirthDate = DateTime.UtcNow,
                Site = "http://localhost"
            };

            Assert.True(author.Id == 0);
            await repo.CreateAuthor(author);
            Assert.True(author.Id > 0);

            var authorFromDb = await libContext.Authors.FirstOrDefaultAsync(a => a.Id == author.Id);
            Assert.NotNull(authorFromDb);
            Assert.True(author.FirstName == authorFromDb.FirstName);
            Assert.True(author.LastName == authorFromDb.LastName);
            Assert.True(author.BirthDate == authorFromDb.BirthDate);
            Assert.True(author.Site == authorFromDb.Site);
        }

        [Fact]
        public async Task DeleteAuthorShouldThroughKeyNotFoundForInvalidId()
        {
            var options = new DbContextOptionsBuilder<LibraryContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            using var libContext = new LibraryContext(options);
            IAuthorRepository repo = new AuthorRepository(libContext);
            await Assert.ThrowsAsync<KeyNotFoundException>(async () => await repo.DeleteAuthor(100));
        }
    }
}
