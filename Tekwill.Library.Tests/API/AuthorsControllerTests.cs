using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Tekwill.Library.Application.Common;
using Tekwill.Library.Application.DTOs.Authors;
using Tekwill.Library.Application.Profiles;
using Tekwill.Library.Controllers;
using Tekwill.Library.Domain.Entities;
using Tekwill.Library.Infrastructure.Data;
using Tekwill.Library.Infrastructure.Persistance;

namespace Tekwill.Library.Tests.API
{
    public class AuthorsControllerTests
    {
        [Fact]
        public async Task GetShouldReturnOkWithEmptyListIfNoAuthorsFound()
        {
            using var libContext = CreateContext();
            var controller = CreateController(libContext);

            var result = await controller.Get(1, 10);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, okResult.StatusCode);
            var paginated = Assert.IsType<PaginatedList<AuthorDto>>(okResult.Value);
            Assert.Empty(paginated.Items);
        }

        [Fact]
        public async Task GetShouldReturnOkWithPagedAuthors()
        {
            using var libContext = CreateContext();
            await libContext.Authors.AddRangeAsync(
                new Author { FirstName = "A1", LastName = "L1", BirthDate = DateTime.UtcNow, Site = "http://a1" },
                new Author { FirstName = "A2", LastName = "L2", BirthDate = DateTime.UtcNow, Site = "http://a2" });
            await libContext.SaveChangesAsync();

            var controller = CreateController(libContext);

            var result = await controller.Get(1, 10);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, okResult.StatusCode);
            var paginated = Assert.IsType<PaginatedList<AuthorDto>>(okResult.Value);
            Assert.Equal(2, paginated.Items.Count);
            Assert.Equal(1, paginated.PageIndex);
            Assert.Contains(paginated.Items, a => a.FirstName == "A1");
        }

        [Fact]
        public async Task GetByIdShouldReturnOkWithAuthor()
        {
            using var libContext = CreateContext();
            var author = new Author
            {
                FirstName = "A1",
                LastName = "L1",
                BirthDate = DateTime.UtcNow,
                Site = "http://a1"
            };
            await libContext.Authors.AddAsync(author);
            await libContext.SaveChangesAsync();

            var controller = CreateController(libContext);

            var result = await controller.Get(author.Id);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, okResult.StatusCode);
            var dto = Assert.IsType<AuthorDto>(okResult.Value);
            Assert.Equal(author.Id, dto.Id);
            Assert.Equal(author.FirstName, dto.FirstName);
            Assert.Equal(author.LastName, dto.LastName);
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

        private static LibraryContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<LibraryContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new LibraryContext(options);
        }

        private static AuthorsController CreateController(LibraryContext libContext)
        {
            var moqValidator = new Mock<IValidator<CreateAuthorDto>>();
            var mapper = new MapperConfiguration(cfg => cfg.AddProfile<AuthorProfile>()).CreateMapper();
            return new AuthorsController(
                NullLogger<AuthorsController>.Instance,
                new AuthorRepository(libContext),
                mapper,
                moqValidator.Object);
        }
    }
}
