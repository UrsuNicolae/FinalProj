using Microsoft.EntityFrameworkCore;
using Tekwill.Library.Application.Common;
using Tekwill.Library.Application.Interfaces;
using Tekwill.Library.Domain.Entities;
using Tekwill.Library.Infrastructure.Data;

namespace Tekwill.Library.Infrastructure.Persistance
{
    public class GenRepository : IGenRepository
    {
        private readonly LibraryContext context;

        public GenRepository(LibraryContext context)
        {
            this.context = context;
        }

        public async Task CreateGen(Gen gen, CancellationToken ct = default)
        {
            await context.Gens.AddAsync(gen, ct);
            await context.SaveChangesAsync(ct);
        }

        public async Task DeleteGen(int id, CancellationToken ct = default)
        {
            var gen = await context.Gens.FirstOrDefaultAsync(a => a.Id == id, ct);
            if (gen == null)
                throw new KeyNotFoundException($"Invalid id:{id}");

            context.Gens.Remove(gen);
            await context.SaveChangesAsync(ct);
        }

        public async Task<Gen> GetGen(int id, CancellationToken ct = default)
        {
            var gen = await context.Gens.FirstOrDefaultAsync(a => a.Id == id, ct);
            if (gen == null)
                throw new KeyNotFoundException($"Invalid id:{id}");
            return gen;
        }

        public async Task<PaginatedList<Gen>> GetGens(int page, int pageSize, CancellationToken ct = default)
        {
            var count = await context.Gens.CountAsync(ct);
            var gens = await context.Gens.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
            return new PaginatedList<Gen>(gens, pageSize, (int)Math.Ceiling((decimal)count / pageSize));
        }

        public async Task UpdateGen(Gen gen, CancellationToken ct = default)
        {
            var gensFromDb = await context.Gens.FirstOrDefaultAsync(a => a.Id == gen.Id, ct);
            if (gensFromDb == null)
                throw new KeyNotFoundException($"Invalid id:{gen.Id}");

            gensFromDb.Name = gen.Name;
            context.Gens.Update(gensFromDb);
            await context.SaveChangesAsync(ct);
        }
    }
}
