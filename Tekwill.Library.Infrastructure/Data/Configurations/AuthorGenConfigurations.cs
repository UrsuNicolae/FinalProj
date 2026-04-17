using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tekwill.Library.Domain.Entities;

namespace Tekwill.Library.Infrastructure.Data.Configurations
{
    public class AuthorGenConfigurations : IEntityTypeConfiguration<AuthorGen>
    {
        public void Configure(EntityTypeBuilder<AuthorGen> builder)
        {
            builder.ToTable("author_gens");
            builder.HasKey(a => new { a.AuthorId, a.GenId });
            builder.HasOne(a => a.Author)
                .WithMany(a => a.AuthorGens)
                .HasForeignKey(a => a.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.Gen)
                .WithMany(a => a.AuthorGens)
                .HasForeignKey(a => a.GenId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
