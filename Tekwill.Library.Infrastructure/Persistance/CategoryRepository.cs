using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;
using Tekwill.Library.Application.Common;
using Tekwill.Library.Application.Interfaces;
using Tekwill.Library.Domain.Entities;
using Tekwill.Library.Infrastructure.Data;

namespace Tekwill.Library.Infrastructure.Persistance
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly LibraryContext context;

        public CategoryRepository(LibraryContext libraryContext)
        {
            this.context = libraryContext;
        }

        public async Task<Category> CreateCategory(Category category, CancellationToken ct = default)
        {
            await context.Categories.AddAsync(category, ct);
            await context.SaveChangesAsync(ct);
            return category;
        }

        public async Task DeleteCategory(int id, CancellationToken ct = default)
        {
            var category = await context.Categories.FirstOrDefaultAsync(a => a.Id == id, ct);
            if (category == null)
                throw new KeyNotFoundException($"Invalid id:{id}");

            context.Categories.Remove(category);
            await context.SaveChangesAsync(ct);
        }

        public async Task<PaginatedList<Category>> GetCategories(int page, int pageSize, CancellationToken ct = default)
        {
            var count = await context.Categories.CountAsync(ct);
            var categories = await context.Categories.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
            return new PaginatedList<Category>(categories, pageSize, (int)Math.Ceiling((decimal)count / pageSize));
        }

        public async Task<Category> GetCategory(int id, CancellationToken ct = default)
        {
            var category = await context.Categories.FirstOrDefaultAsync(a => a.Id == id, ct);
            if (category == null)
                throw new KeyNotFoundException($"Invalid id:{id}");
            return category;
        }

        public async Task UpdateCategory(Category category, CancellationToken ct = default)
        {
            var categoryFromDb = await context.Categories.FirstOrDefaultAsync(a => a.Id == category.Id, ct);
            if (categoryFromDb == null)
                throw new KeyNotFoundException($"Invalid id:{category.Id}");

            categoryFromDb.Name = category.Name;
            context.Categories.Update(categoryFromDb);
            await context.SaveChangesAsync(ct);
        }
    }
}
