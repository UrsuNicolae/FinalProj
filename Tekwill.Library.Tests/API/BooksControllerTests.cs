using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Tekwill.Library.Application.Common;
using Tekwill.Library.Application.DTOs.Books;
using Tekwill.Library.Application.Profiles;
using Tekwill.Library.Controllers;
using Tekwill.Library.Domain.Entities;
using Tekwill.Library.Infrastructure.Data;
using Tekwill.Library.Infrastructure.Persistance;

namespace Tekwill.Library.Tests.API
{
    public class BooksControllerTests
    {
        [Fact]
        public async Task GetShouldReturnOkWithEmptyListIfNoBooksFound()
        {
            using var libContext = CreateContext();
            var controller = CreateController(libContext);

            var result = await controller.Get(1, 10);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, okResult.StatusCode);
            var paginated = Assert.IsType<PaginatedList<BookDto>>(okResult.Value);
            Assert.Empty(paginated.Items);
        }

        [Fact]
        public async Task GetShouldReturnOkWithPagedBooks()
        {
            using var libContext = CreateContext();
            var (authorId, categoryId) = await SeedAuthorAndCategory(libContext);

            await libContext.Books.AddRangeAsync(
                new Book { Title = "T1", ISBN = "111", AuthorId = authorId, CategoryId = categoryId },
                new Book { Title = "T2", ISBN = "222", AuthorId = authorId, CategoryId = categoryId });
            await libContext.SaveChangesAsync();

            var controller = CreateController(libContext);

            var result = await controller.Get(1, 10);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, okResult.StatusCode);
            var paginated = Assert.IsType<PaginatedList<BookDto>>(okResult.Value);
            Assert.Equal(2, paginated.Items.Count);
            Assert.Equal(1, paginated.PageIndex);
            Assert.Contains(paginated.Items, b => b.Title == "T1");
        }

        [Fact]
        public async Task GetByIdShouldReturnOkWithBook()
        {
            using var libContext = CreateContext();
            var (authorId, categoryId) = await SeedAuthorAndCategory(libContext);

            var book = new Book { Title = "T1", ISBN = "111", AuthorId = authorId, CategoryId = categoryId };
            await libContext.Books.AddAsync(book);
            await libContext.SaveChangesAsync();

            var controller = CreateController(libContext);

            var result = await controller.Get(book.Id);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, okResult.StatusCode);
            var dto = Assert.IsType<BookDto>(okResult.Value);
            Assert.Equal(book.Id, dto.Id);
            Assert.Equal(book.Title, dto.Title);
            Assert.Equal(book.ISBN, dto.ISBN);
        }

        [Fact]
        public async Task GetByIdShouldReturnNotFoundForInvalidId()
        {
            using var libContext = CreateContext();
            var controller = CreateController(libContext);

            var result = await controller.Get(999);

            var notFound = Assert.IsType<NotFoundResult>(result.Result);
            Assert.Equal(404, notFound.StatusCode);
        }

        private static async Task<(int authorId, int categoryId)> SeedAuthorAndCategory(LibraryContext libContext)
        {
            var author = new Author
            {
                FirstName = "A",
                LastName = "B",
                BirthDate = DateTime.UtcNow,
                Site = "http://localhost"
            };
            var category = new Category { Name = "Cat" };
            await libContext.Authors.AddAsync(author);
            await libContext.Categories.AddAsync(category);
            await libContext.SaveChangesAsync();
            return (author.Id, category.Id);
        }

        private static LibraryContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<LibraryContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new LibraryContext(options);
        }

        private static BooksController CreateController(LibraryContext libContext)
        {
            var moqValidator = new Mock<IValidator<CreateBookDto>>();
            var mapper = new MapperConfiguration(cfg => cfg.AddProfile<BookProfile>()).CreateMapper();
            return new BooksController(
                NullLogger<BooksController>.Instance,
                new BookRepository(libContext),
                mapper,
                moqValidator.Object);
        }
    }
}
