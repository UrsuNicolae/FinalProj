using Microsoft.EntityFrameworkCore;
using Tekwill.Library.Application.Interfaces;
using Tekwill.Library.Domain.Entities;
using Tekwill.Library.Infrastructure.Data;
using Tekwill.Library.Infrastructure.Persistance;

namespace Tekwill.Library.Tests.Infrastracture
{
    public class BookRepositoryTests
    {
        [Fact]
        public async Task CreateBookShouldSaveInDb()
        {
            using var libContext = await SetUpLibContext();
            IBookRepository repo = new BookRepository(libContext);

            var book = new Book
            {
                ISBN = "TestIsb",
                Title = "TestTile",
                AuthorId = 1,
                CategoryId = 1
            };

            Assert.True(book.Id == 0);
            await repo.CreateBook(book);
            Assert.True(book.Id > 0);

            var bookFromDb = await libContext.Books.FirstOrDefaultAsync(a => a.Id == book.Id);
            Assert.NotNull(bookFromDb);
            Assert.True(book.ISBN == bookFromDb.ISBN);
            Assert.True(book.Title == bookFromDb.Title);
            Assert.True(book.AuthorId == bookFromDb.AuthorId);
            Assert.True(book.CategoryId == bookFromDb.CategoryId);
        }

        [Fact]
        public async Task DeleteBookShouldThroughKeyNotFoundForInvalidId()
        {
            using var libContext = await SetUpLibContext();
            IBookRepository repo = new BookRepository(libContext);
            await Assert.ThrowsAsync<KeyNotFoundException>(async () => await repo.DeleteBook(100));
        }

        [Fact]
        public async Task DeleteBookShouldRemoveBookFromDb()
        {
            using var libContext = await SetUpLibContext();
            IBookRepository repo = new BookRepository(libContext);
            var book = new Book
            {
                ISBN = "TestIsb",
                Title = "TestTile",
                AuthorId = 1,
                CategoryId = 1
            };

            await libContext.AddAsync(book);
            await libContext.SaveChangesAsync();

            Assert.True(libContext.Books.Any(a => a.Id == book.Id));
            await repo.DeleteBook(book.Id);
            Assert.False(libContext.Books.Any(a => a.Id == book.Id));
        }

        [Fact]
        public async Task GetBookShouldThroughKeyNotFoundForInvalidId()
        {
            IBookRepository repo = new BookRepository(await SetUpLibContext());
            await Assert.ThrowsAsync<KeyNotFoundException>(async () => await repo.GetBook(100));
        }

        [Fact]
        public async Task GetBookShouldReturnBookFromDb()
        {
            using var libContext = await SetUpLibContext();
            IBookRepository repo = new BookRepository(libContext);
            var book = new Book
            {
                ISBN = "TestIsb",
                Title = "TestTile",
                AuthorId = 1,
                CategoryId = 1
            };

            await libContext.AddAsync(book);
            await libContext.SaveChangesAsync();

            Assert.True(libContext.Books.Any(a => a.Id == book.Id));
            var bookFromDb = await repo.GetBook(book.Id);
            Assert.NotNull(bookFromDb);
            Assert.Equal(book.Id, bookFromDb.Id);
            Assert.Equal(book.ISBN, bookFromDb.ISBN);
            Assert.Equal(book.Title, bookFromDb.Title);
            Assert.Equal(book.AuthorId, bookFromDb.AuthorId);
            Assert.Equal(book.CategoryId, bookFromDb.CategoryId);
        }

        [Fact]
        public async Task GetBooksShouldReturnEmptyListIfNoBooksFound()
        {
            IBookRepository repo = new BookRepository(await SetUpLibContext());
            var books = await repo.GetBooks(1, 10);
            Assert.True(books.Items.Count == 0);
        }

        [Fact]
        public async Task GetBooksShouldReturnsPagedBooks()
        {
            using var libContext = await SetUpLibContext();
            var books = new List<Book>
            {
                new Book
                {
                    ISBN = "TestIsb",
                    Title = "TestTile",
                    AuthorId = 1,
                    CategoryId = 1
                },
                new Book
                {
                    ISBN = "TestIsb",
                    Title = "TestTile",
                    AuthorId = 1,
                    CategoryId = 1
                },
                new Book
                {
                    ISBN = "TestIsb",
                    Title = "TestTile",
                    AuthorId = 1,
                    CategoryId = 1
                }
            };

            await libContext.AddRangeAsync(books);
            await libContext.SaveChangesAsync();


            IBookRepository repo = new BookRepository(libContext);
            var booksFromDb = await repo.GetBooks(1, 10);
            Assert.True(booksFromDb.Items.Count != 0);
        }

        [Fact]
        public async Task UpdateBookShouldThroughKeyNotFoundForInvalidId()
        {
            using var libContext = await SetUpLibContext();
            var bookToUpdate = new Book
            {
                Id = 10,
                ISBN = "TestUpdated"
            };

            IBookRepository repo = new BookRepository(libContext);
            await Assert.ThrowsAsync<KeyNotFoundException>(async () => await repo.UpdateBook(bookToUpdate));
        }

        [Fact]
        public async Task UpdateBookShouldUpdateActualValueInDb()
        {
            using var libContext = await SetUpLibContext();
            var seedBook = new Book
            {
                ISBN = "TestIsb",
                Title = "TestTile",
                AuthorId = 1,
                CategoryId = 1
            };
            await libContext.Books.AddAsync(seedBook);
            await libContext.SaveChangesAsync();
            libContext.ChangeTracker.Clear();

            var bookToUpdate = new Book
            {
                Id = seedBook.Id,
                ISBN = "UpdatedISBN",
                Title = "UpdateTitle"
            };

            IBookRepository repo = new BookRepository(libContext);
            var bookFromDb = await libContext.Books.AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == seedBook.Id);
            Assert.NotNull(bookFromDb);
            Assert.Equal(seedBook.Id, bookFromDb.Id);
            Assert.Equal(seedBook.ISBN, bookFromDb.ISBN);
            Assert.Equal(seedBook.Title, bookFromDb.Title);
            Assert.Equal(seedBook.AuthorId, bookFromDb.AuthorId);
            Assert.Equal(seedBook.CategoryId, bookFromDb.CategoryId);

            await repo.UpdateBook(bookToUpdate);
            bookFromDb = await libContext.Books.AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == seedBook.Id);
            Assert.NotNull(bookFromDb);
            Assert.Equal(bookToUpdate.Id, bookFromDb.Id);
            Assert.Equal(bookToUpdate.AuthorId, bookFromDb.AuthorId);
            Assert.Equal(bookToUpdate.ISBN, bookFromDb.ISBN);
            Assert.Equal(bookToUpdate.Title, bookFromDb.Title);
            Assert.Equal(bookToUpdate.CategoryId, bookFromDb.CategoryId);
        }

        private async Task<LibraryContext> SetUpLibContext()
        {
            var options = new DbContextOptionsBuilder<LibraryContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var libContext = new LibraryContext(options);
            var author = new Author
            {
                FirstName = "TestName",
                LastName = "TestLastName",
                BirthDate = DateTime.UtcNow,
                Site = "http://localhost"
            };
            await libContext.Authors.AddAsync(author);
            var category = new Category
            {
                Name = "TestCategory"
            };
            await libContext.Categories.AddAsync(category);
            await libContext.SaveChangesAsync();
            return libContext;
        }
    }
}
