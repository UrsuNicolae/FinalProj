using Tekwill.Library.Application.Common;
using Tekwill.Library.Application.Interfaces;
using Tekwill.Library.Domain.Entities;

namespace Tekwill.Library.Infrastructure.Persistance
{
    public class GenRepository : IGenRepository
    {
        public Task CreateGen(Gen gen, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task DeleteGen(int id, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<Gen> GetGen(int id, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<PaginatedList<Gen>> GetGens(int page, int pageSize, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task UpdateGen(Gen gen, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
    }
}
