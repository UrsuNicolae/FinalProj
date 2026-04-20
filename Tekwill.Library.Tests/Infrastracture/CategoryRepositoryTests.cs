using Microsoft.EntityFrameworkCore;
using Tekwill.Library.Application.Interfaces;
using Tekwill.Library.Domain.Entities;
using Tekwill.Library.Infrastructure.Data;
using Tekwill.Library.Infrastructure.Persistance;

namespace Tekwill.Library.Tests.Infrastracture
{
    public class CategoryRepositoryTests
    {
        [Fact]
        public async Task CreateCategoryShouldSaveInDb()
        {
            using var libContext = await SetUpLibContext();
            ICategoryRepository repo = new CategoryRepository(libContext);

            var category = new Category
            {
                Name = "TestCategory"
            };

            Assert.True(category.Id == 0);
            await repo.CreateCategory(category);
            Assert.True(category.Id > 0);

            var categoryFromDb = await libContext.Categories.FirstOrDefaultAsync(a => a.Id == category.Id);
            Assert.NotNull(categoryFromDb);
            Assert.True(category.Name == categoryFromDb.Name);
        }

        [Fact]
        public async Task DeleteCategoryShouldThroughKeyNotFoundForInvalidId()
        {
            using var libContext = await SetUpLibContext();
            ICategoryRepository repo = new CategoryRepository(libContext);
            await Assert.ThrowsAsync<KeyNotFoundException>(async () => await repo.DeleteCategory(100));
        }

        [Fact]
        public async Task DeleteCategoryShouldRemoveCategoryFromDb()
        {
            using var libContext = await SetUpLibContext();
            ICategoryRepository repo = new CategoryRepository(libContext);
            var category = new Category
            {
                Name = "TestGet"
            };

            await libContext.AddAsync(category);
            await libContext.SaveChangesAsync();

            Assert.True(libContext.Categories.Any(a => a.Id == category.Id));
            await repo.DeleteCategory(category.Id);
            Assert.False(libContext.Categories.Any(a => a.Id == category.Id));
        }

        [Fact]
        public async Task GetCategoryShouldThroughKeyNotFoundForInvalidId()
        {
            ICategoryRepository repo = new CategoryRepository(await SetUpLibContext());
            await Assert.ThrowsAsync<KeyNotFoundException>(async () => await repo.GetCategory(100));
        }

        [Fact]
        public async Task GetCategoryShouldReturnCategoryFromDb()
        {
            using var libContext = await SetUpLibContext();
            ICategoryRepository repo = new CategoryRepository(libContext);
            var category = new Category
            {
                Name = "TestGet"
            };

            await libContext.AddAsync(category);
            await libContext.SaveChangesAsync();

            Assert.True(libContext.Categories.Any(a => a.Id == category.Id));
            var categoryFromDb = await repo.GetCategory(category.Id);
            Assert.NotNull(categoryFromDb);
            Assert.Equal(category.Id, categoryFromDb.Id);
            Assert.Equal(category.Name, categoryFromDb.Name);
        }

        [Fact]
        public async Task GetCategoriesShouldReturnEmptyListIfNoCategoriesFound()
        {
            ICategoryRepository repo = new CategoryRepository(await SetUpLibContext());
            var categories = await repo.GetCategories(1, 10);
            Assert.True(categories.Items.Count == 0);
        }

        [Fact]
        public async Task GetCategoriesShouldReturnsPagedCategories()
        {
            using var libContext = await SetUpLibContext();
            var categories = new List<Category>
            {
                new Category
                {
                    Name = "TestGet"
                },
                new Category
                {
                    Name = "TestGet"
                },
                new Category
                {
                    Name = "TestGet"
                }
            };

            await libContext.AddRangeAsync(categories);
            await libContext.SaveChangesAsync();


            ICategoryRepository repo = new CategoryRepository(libContext);
            var categoriesFromDb = await repo.GetCategories(1, 10);
            Assert.True(categoriesFromDb.Items.Count != 0);
        }

        [Fact]
        public async Task UpdateCategoryShouldThroughKeyNotFoundForInvalidId()
        {
            using var libContext = await SetUpLibContext();
            var categoryToUpdate = new Category
            {
                Id = 10,
                Name = "TestUpdated"
            };

            ICategoryRepository repo = new CategoryRepository(libContext);
            await Assert.ThrowsAsync<KeyNotFoundException>(async () => await repo.UpdateCategory(categoryToUpdate));
        }

        [Fact]
        public async Task UpdateCategoryShouldUpdateActualValueInDb()
        {
            using var libContext = await SetUpLibContext();
            var seedCategory = new Category
            {
                Name = "TestGet"
            };
            await libContext.Categories.AddAsync(seedCategory);
            await libContext.SaveChangesAsync();
            libContext.ChangeTracker.Clear();

            var categoryToUpdate = new Category
            {
                Id = seedCategory.Id,
                Name = "UpdatedName"
            };

            ICategoryRepository repo = new CategoryRepository(libContext);
            var categoryFromDb = await libContext.Categories.AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == seedCategory.Id);
            Assert.NotNull(categoryFromDb);
            Assert.Equal(seedCategory.Id, categoryFromDb.Id);
            Assert.Equal(seedCategory.Name, categoryFromDb.Name);

            await repo.UpdateCategory(categoryToUpdate);
            categoryFromDb = await libContext.Categories.AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == seedCategory.Id);
            Assert.NotNull(categoryFromDb);
            Assert.Equal(categoryToUpdate.Id, categoryFromDb.Id);
            Assert.Equal(categoryToUpdate.Name, categoryFromDb.Name);
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
