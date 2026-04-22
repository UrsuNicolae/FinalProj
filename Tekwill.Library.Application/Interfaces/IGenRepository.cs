using Tekwill.Library.Application.Common;
using Tekwill.Library.Domain.Entities;

namespace Tekwill.Library.Application.Interfaces
{
    public interface IGenRepository
    {

        Task<PaginatedList<Gen>> GetGens(int page, int pageSize, CancellationToken ct = default);
        Task<Gen> GetGen(int id, CancellationToken ct = default);

        Task<Gen> CreateGen(Gen gen, CancellationToken ct = default);
        Task UpdateGen(Gen gen, CancellationToken ct = default);
        Task DeleteGen(int id, CancellationToken ct = default);
    }
}
