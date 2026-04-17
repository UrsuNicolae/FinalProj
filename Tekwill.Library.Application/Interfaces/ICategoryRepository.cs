using Tekwill.Library.Application.Common;
using Tekwill.Library.Domain.Entities;

namespace Tekwill.Library.Application.Interfaces
{
    public interface ICategoryRepository
    {
        Task<PaginatedList<Category>> GetCategories(int page, int pageSize, CancellationToken ct = default);
        Task<Category> GetCategory(int id, CancellationToken ct = default);

        Task CreateCategory(Category category, CancellationToken ct = default);
        Task UpdateCategory(Category category, CancellationToken ct = default);
        Task DeleteCategory(int id, CancellationToken ct = default);
    }
}
