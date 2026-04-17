using Microsoft.EntityFrameworkCore;
using Tekwill.Library.Domain.Entities;

namespace Tekwill.Library.Infrastructure.Data
{
    public class LibraryContext : DbContext
    {
        public DbSet<Author> Authors { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<AuthorGen> AuthorGens { get; set; }
        public DbSet<Gen> Gens { get; set; }
        public LibraryContext(DbContextOptions<LibraryContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(LibraryContext).Assembly);
        }
    }
}
