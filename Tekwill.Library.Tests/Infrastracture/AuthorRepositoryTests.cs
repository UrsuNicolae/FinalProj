using Microsoft.EntityFrameworkCore;
using Tekwill.Library.Application.Interfaces;
using Tekwill.Library.Domain.Entities;
using Tekwill.Library.Infrastructure.Data;
using Tekwill.Library.Infrastructure.Persistance;

namespace Tekwill.Library.Tests.Infrastracture
{
    public class AuthorRepositoryTests
    {
        [Fact]
        public async Task CreateAuthorShouldSaveInDb()
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

        [Fact]
        public async Task DeleteAuthorShouldRemoveAuthorFromDb()
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

            await libContext.AddAsync(author);
            await libContext.SaveChangesAsync();

            Assert.True(libContext.Authors.Any(a => a.Id == author.Id));
            await repo.DeleteAuthor(author.Id);
            Assert.False(libContext.Authors.Any(a => a.Id == author.Id));
        }

        [Fact]
        public async Task GetAuthorShouldThroughKeyNotFoundForInvalidId()
        {
            var options = new DbContextOptionsBuilder<LibraryContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            using var libContext = new LibraryContext(options);
            IAuthorRepository repo = new AuthorRepository(libContext);
            await Assert.ThrowsAsync<KeyNotFoundException>(async () => await repo.GetAuthor(100));
        }

        [Fact]
        public async Task GetAuthorShouldReturnAuthorFromDb()
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

            await libContext.AddAsync(author);
            await libContext.SaveChangesAsync();

            Assert.True(libContext.Authors.Any(a => a.Id == author.Id));
            var authorFromDb = await repo.GetAuthor(author.Id);
            Assert.NotNull(authorFromDb);
            Assert.Equal(author.Id, authorFromDb.Id);
            Assert.Equal(author.FirstName, authorFromDb.FirstName);
            Assert.Equal(author.BirthDate, authorFromDb.BirthDate);
            Assert.Equal(author.Site, authorFromDb.Site);
        }

        [Fact]
        public async Task GetAuthorsShouldReturnEmptyListIfNoAuthorsFound()
        {
            var options = new DbContextOptionsBuilder<LibraryContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            using var libContext = new LibraryContext(options);
            IAuthorRepository repo = new AuthorRepository(libContext);
            var authors = await repo.GetAuthors(1, 10);
            Assert.True(authors.Items.Count == 0);
        }

        [Fact]
        public async Task GetAuthorsShouldReturnsPagedAuthors()
        {
            var options = new DbContextOptionsBuilder<LibraryContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            using var libContext = new LibraryContext(options);
            var authors = new List<Author>
            {
                new Author
                {
                    FirstName = "TestName",
                    LastName = "TestLastName",
                    BirthDate = DateTime.UtcNow,
                    Site = "http://localhost"
                },
                new Author
                {
                    FirstName = "TestName",
                    LastName = "TestLastName",
                    BirthDate = DateTime.UtcNow,
                    Site = "http://localhost"
                },
                new Author
                {
                    FirstName = "TestName",
                    LastName = "TestLastName",
                    BirthDate = DateTime.UtcNow,
                    Site = "http://localhost"
                }
            };

            await libContext.AddRangeAsync(authors);
            await libContext.SaveChangesAsync();


            IAuthorRepository repo = new AuthorRepository(libContext);
            var authorsFromDb = await repo.GetAuthors(1, 10);
            Assert.True(authorsFromDb.Items.Count != 0);
        }

        [Fact]
        public async Task UpdateAuthorShouldThroughKeyNotFoundForInvalidId()
        {
            var options = new DbContextOptionsBuilder<LibraryContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            using var libContext = new LibraryContext(options);
            var authorToUpdate = new Author
            {
                Id = 10,
                FirstName = "TestUpdated"
            };

            IAuthorRepository repo = new AuthorRepository(libContext);
            await Assert.ThrowsAsync<KeyNotFoundException>(async () => await repo.UpdateAuthor(authorToUpdate));
        }

        [Fact]
        public async Task UpdateAuthorShouldUpdateActualValueInDb()
        {
            var options = new DbContextOptionsBuilder<LibraryContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            using var libContext = new LibraryContext(options);
            var seedAuthor = new Author
            {
                FirstName = "TestName",
                LastName = "TestLastName",
                BirthDate = DateTime.UtcNow,
                Site = "http://localhost"
            };
            await libContext.Authors.AddAsync(seedAuthor);
            await libContext.SaveChangesAsync();
            libContext.ChangeTracker.Clear();

            var authorToUpdate = new Author
            {
                Id = seedAuthor.Id,
                FirstName = "Updated",
                LastName = "Updated",
                BirthDate = DateTime.UtcNow - TimeSpan.FromDays(1),
                Site = "http://localhostupdated"
            };

            IAuthorRepository repo = new AuthorRepository(libContext);
            var authorFromDb =  await libContext.Authors.AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == seedAuthor.Id);
            Assert.NotNull(authorFromDb);
            Assert.Equal(seedAuthor.Id, authorFromDb.Id);
            Assert.Equal(seedAuthor.FirstName, authorFromDb.FirstName);
            Assert.Equal(seedAuthor.LastName, authorFromDb.LastName);
            Assert.Equal(seedAuthor.BirthDate, authorFromDb.BirthDate);
            Assert.Equal(seedAuthor.Site, authorFromDb.Site);

            await repo.UpdateAuthor(authorToUpdate);
            authorFromDb = await libContext.Authors.AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == seedAuthor.Id);
            Assert.NotNull(authorFromDb);
            Assert.Equal(authorToUpdate.Id, authorFromDb.Id);
            Assert.Equal(authorToUpdate.FirstName, authorFromDb.FirstName);
            Assert.Equal(authorToUpdate.LastName, authorFromDb.LastName);
            Assert.Equal(authorToUpdate.BirthDate, authorFromDb.BirthDate);
            Assert.Equal(authorToUpdate.Site, authorFromDb.Site);
        }
    }
}
