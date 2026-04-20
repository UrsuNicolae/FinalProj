using Microsoft.EntityFrameworkCore;
using Tekwill.Library.Application.Interfaces;
using Tekwill.Library.Domain.Entities;
using Tekwill.Library.Infrastructure.Data;
using Tekwill.Library.Infrastructure.Persistance;

namespace Tekwill.Library.Tests.Infrastracture
{
    public class GenRepositoryTests
    {
        [Fact]
        public async Task CreateGenShouldSaveInDb()
        {
            using var libContext = await SetUpLibContext();
            IGenRepository repo = new GenRepository(libContext);

            var gen = new Gen
            {
                Name = "TestGen"
            };

            Assert.True(gen.Id == 0);
            await repo.CreateGen(gen);
            Assert.True(gen.Id > 0);

            var genFromDb = await libContext.Gens.FirstOrDefaultAsync(a => a.Id == gen.Id);
            Assert.NotNull(genFromDb);
            Assert.True(gen.Name == genFromDb.Name);
        }

        [Fact]
        public async Task DeleteGenShouldThroughKeyNotFoundForInvalidId()
        {
            using var libContext = await SetUpLibContext();
            IGenRepository repo = new GenRepository(libContext);
            await Assert.ThrowsAsync<KeyNotFoundException>(async () => await repo.DeleteGen(100));
        }

        [Fact]
        public async Task DeleteGenShouldRemoveGenFromDb()
        {
            using var libContext = await SetUpLibContext();
            IGenRepository repo = new GenRepository(libContext);
            var gen = new Gen
            {
                Name = "TestGet"
            };

            await libContext.AddAsync(gen);
            await libContext.SaveChangesAsync();

            Assert.True(libContext.Gens.Any(a => a.Id == gen.Id));
            await repo.DeleteGen(gen.Id);
            Assert.False(libContext.Gens.Any(a => a.Id == gen.Id));
        }

        [Fact]
        public async Task GetGenShouldThroughKeyNotFoundForInvalidId()
        {
            IGenRepository repo = new GenRepository(await SetUpLibContext());
            await Assert.ThrowsAsync<KeyNotFoundException>(async () => await repo.GetGen(100));
        }

        [Fact]
        public async Task GetGenShouldReturnGenFromDb()
        {
            using var libContext = await SetUpLibContext();
            IGenRepository repo = new GenRepository(libContext);
            var gen = new Gen
            {
                Name = "TestGet"
            };

            await libContext.AddAsync(gen);
            await libContext.SaveChangesAsync();

            Assert.True(libContext.Gens.Any(a => a.Id == gen.Id));
            var genFromDb = await repo.GetGen(gen.Id);
            Assert.NotNull(genFromDb);
            Assert.Equal(gen.Id, genFromDb.Id);
            Assert.Equal(gen.Name, genFromDb.Name);
        }

        [Fact]
        public async Task GetGensShouldReturnEmptyListIfNoGensFound()
        {
            IGenRepository repo = new GenRepository(await SetUpLibContext());
            var gens = await repo.GetGens(1, 10);
            Assert.True(gens.Items.Count == 0);
        }

        [Fact]
        public async Task GetGensShouldReturnsPagedGens()
        {
            using var libContext = await SetUpLibContext();
            var gens = new List<Gen>
            {
                new Gen
                {
                    Name = "TestGet"
                },
                new Gen
                {
                    Name = "TestGet"
                },
                new Gen
                {
                    Name = "TestGet"
                }
            };

            await libContext.AddRangeAsync(gens);
            await libContext.SaveChangesAsync();


            IGenRepository repo = new GenRepository(libContext);
            var gensFromDb = await repo.GetGens(1, 10);
            Assert.True(gensFromDb.Items.Count != 0);
        }

        [Fact]
        public async Task UpdateGenShouldThroughKeyNotFoundForInvalidId()
        {
            using var libContext = await SetUpLibContext();
            var genToUpdate = new Gen
            {
                Id = 10,
                Name = "TestUpdated"
            };

            IGenRepository repo = new GenRepository(libContext);
            await Assert.ThrowsAsync<KeyNotFoundException>(async () => await repo.UpdateGen(genToUpdate));
        }

        [Fact]
        public async Task UpdateGenShouldUpdateActualValueInDb()
        {
            using var libContext = await SetUpLibContext();
            var seedGen = new Gen
            {
                Name = "TestGet"
            };
            await libContext.Gens.AddAsync(seedGen);
            await libContext.SaveChangesAsync();
            libContext.ChangeTracker.Clear();

            var genToUpdate = new Gen
            {
                Id = seedGen.Id,
                Name = "UpdatedName"
            };

            IGenRepository repo = new GenRepository(libContext);
            var genFromDb = await libContext.Gens.AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == seedGen.Id);
            Assert.NotNull(genFromDb);
            Assert.Equal(seedGen.Id, genFromDb.Id);
            Assert.Equal(seedGen.Name, genFromDb.Name);

            await repo.UpdateGen(genToUpdate);
            genFromDb = await libContext.Gens.AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == seedGen.Id);
            Assert.NotNull(genFromDb);
            Assert.Equal(genToUpdate.Id, genFromDb.Id);
            Assert.Equal(genToUpdate.Name, genFromDb.Name);
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
            await libContext.SaveChangesAsync();
            return libContext;
        }
    }
}
