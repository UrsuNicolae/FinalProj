using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Tekwill.Library.Application.Common;
using Tekwill.Library.Application.DTOs.Categories;
using Tekwill.Library.Application.Profiles;
using Tekwill.Library.Controllers;
using Tekwill.Library.Domain.Entities;
using Tekwill.Library.Infrastructure.Data;
using Tekwill.Library.Infrastructure.Persistance;

namespace Tekwill.Library.Tests.API
{
    public class CategoriesControllerTests
    {
        [Fact]
        public async Task GetShouldReturnOkWithEmptyListIfNoCategoriesFound()
        {
            using var libContext = CreateContext();
            var controller = CreateController(libContext);

            var result = await controller.Get(1, 10);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, okResult.StatusCode);
            var paginated = Assert.IsType<PaginatedList<CategoryDto>>(okResult.Value);
            Assert.Empty(paginated.Items);
        }

        [Fact]
        public async Task GetShouldReturnOkWithPagedCategories()
        {
            using var libContext = CreateContext();
            await libContext.Categories.AddRangeAsync(
                new Category { Name = "Fiction" },
                new Category { Name = "Science" });
            await libContext.SaveChangesAsync();

            var controller = CreateController(libContext);

            var result = await controller.Get(1, 10);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, okResult.StatusCode);
            var paginated = Assert.IsType<PaginatedList<CategoryDto>>(okResult.Value);
            Assert.Equal(2, paginated.Items.Count);
            Assert.Equal(1, paginated.PageIndex);
            Assert.Contains(paginated.Items, c => c.Name == "Fiction");
        }

        [Fact]
        public async Task GetByIdShouldReturnOkWithCategory()
        {
            using var libContext = CreateContext();
            var category = new Category { Name = "Fiction" };
            await libContext.Categories.AddAsync(category);
            await libContext.SaveChangesAsync();

            var controller = CreateController(libContext);

            var result = await controller.Get(category.Id);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, okResult.StatusCode);
            var dto = Assert.IsType<CategoryDto>(okResult.Value);
            Assert.Equal(category.Id, dto.Id);
            Assert.Equal(category.Name, dto.Name);
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

        private static CategoriesController CreateController(LibraryContext libContext)
        {
            var mocValidator = new Mock<IValidator<CreateCategoryDto>>();
            var mapper = new MapperConfiguration(cfg => cfg.AddProfile<CategoryProfile>()).CreateMapper();
            return new CategoriesController(
                NullLogger<CategoriesController>.Instance,
                new CategoryRepository(libContext),
                mapper,
                mocValidator.Object);
        }
    }
}
