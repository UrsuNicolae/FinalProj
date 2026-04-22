using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Tekwill.Library.Application.Common;
using Tekwill.Library.Application.DTOs.Gens;
using Tekwill.Library.Application.Profiles;
using Tekwill.Library.Controllers;
using Tekwill.Library.Domain.Entities;
using Tekwill.Library.Infrastructure.Data;
using Tekwill.Library.Infrastructure.Persistance;

namespace Tekwill.Library.Tests.API
{
    public class GensControllerTests
    {
        [Fact]
        public async Task GetShouldReturnOkWithEmptyListIfNoGensFound()
        {
            using var libContext = CreateContext();
            var controller = CreateController(libContext);

            var result = await controller.Get(1, 10);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, okResult.StatusCode);
            var paginated = Assert.IsType<PaginatedList<GenDto>>(okResult.Value);
            Assert.Empty(paginated.Items);
        }

        [Fact]
        public async Task GetShouldReturnOkWithPagedGens()
        {
            using var libContext = CreateContext();
            await libContext.Gens.AddRangeAsync(
                new Gen { Name = "Fiction" },
                new Gen { Name = "Non-Fiction" },
                new Gen { Name = "Sci-Fi" });
            await libContext.SaveChangesAsync();

            var controller = CreateController(libContext);

            var result = await controller.Get(1, 10);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, okResult.StatusCode);
            var paginated = Assert.IsType<PaginatedList<GenDto>>(okResult.Value);
            Assert.Equal(3, paginated.Items.Count);
            Assert.Equal(1, paginated.PageIndex);
        }

        [Fact]
        public async Task GetByIdShouldReturnOkWithGen()
        {
            using var libContext = CreateContext();
            var gen = new Gen { Name = "Fiction" };
            await libContext.Gens.AddAsync(gen);
            await libContext.SaveChangesAsync();

            var controller = CreateController(libContext);

            var result = await controller.Get(gen.Id);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, okResult.StatusCode);
            var dto = Assert.IsType<GenDto>(okResult.Value);
            Assert.Equal(gen.Id, dto.Id);
            Assert.Equal(gen.Name, dto.Name);
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

        [Fact]
        public async Task CreateShouldReturnBadRequestForInvalidGen()
        {
            using var libContext = CreateContext();
            var validator = new Mock<IValidator<CreateGenDto>>();
            var validationResult = new ValidationResult(new List<ValidationFailure> { new ValidationFailure(nameof(CreateGenDto.Name), "Invalid name")});
            validator.Setup(s => s.ValidateAsync(It.IsAny<CreateGenDto>(), It.IsAny<CancellationToken>())).ReturnsAsync(validationResult);
            var controller = CreateController(libContext, validator);
            var genToCreate = new CreateGenDto
            {
                Name = "Invalid Name"
            };

            var result = await controller.Create(genToCreate);

            var badResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(400, badResult.StatusCode);
        }
        [Fact]
        public async Task CreateShouldPersistGenInDbIfGenIsValid()
        {
            using var libContext = CreateContext();
            var validator = new Mock<IValidator<CreateGenDto>>();
            var validationResult = new ValidationResult(new List<ValidationFailure> ());
            validator.Setup(s => s.ValidateAsync(It.IsAny<CreateGenDto>(), It.IsAny<CancellationToken>())).ReturnsAsync(validationResult);
            var controller = CreateController(libContext, validator);
            var genToCreate = new CreateGenDto
            {
                Name = "Valid Name"
            };

            var result = await controller.Create(genToCreate);

            var okResult = Assert.IsType<CreatedAtRouteResult>(result.Result);
            Assert.Equal(201, okResult.StatusCode);
            var createdGen = Assert.IsType<GenDto>(okResult.Value);
            Assert.Equal(genToCreate.Name, createdGen.Name);
            Assert.True(createdGen.Id > 0);
        }

        private static LibraryContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<LibraryContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new LibraryContext(options);
        }

        private static GensController CreateController(LibraryContext libContext,
            Mock<IValidator<CreateGenDto>>? validator = null)
        {
            var mapper = new MapperConfiguration(cfg => cfg.AddProfile<GenProfile>()).CreateMapper();
            var moqValidator = validator ?? new Mock<IValidator<CreateGenDto>>();
            return new GensController(
                NullLogger<GensController>.Instance,
                new GenRepository(libContext),
                mapper,
                moqValidator.Object);
        }
    }
}
