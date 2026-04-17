using Tekwill.Library.Application.Common;
using Tekwill.Library.Application.Interfaces;

namespace Tekwill.Library.Infrastructure.Persistance
{
    public class Category : ICategoryRepository
    {
        public Task CreateCategory(Domain.Entities.Category category, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task DeleteCategory(int id, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<PaginatedList<Domain.Entities.Category>> GetCategories(int page, int pageSize, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<Domain.Entities.Category> GetCategory(int id, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task UpdateCategory(Domain.Entities.Category category, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
    }
}
