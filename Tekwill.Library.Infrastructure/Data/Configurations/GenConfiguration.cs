using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tekwill.Library.Domain.Entities;

namespace Tekwill.Library.Infrastructure.Data.Configurations
{
    public class GenConfiguration : IEntityTypeConfiguration<Gen>
    {
        public void Configure(EntityTypeBuilder<Gen> builder)
        {
            builder.ToTable("genres");
            builder.HasKey(g => g.Id);
            builder.Property(g => g.Id)
                .ValueGeneratedOnAdd();
            builder.Property(g => g.Name)
                .HasMaxLength(50)
                .IsRequired();
            builder.HasMany(g => g.AuthorGens)
                .WithOne(a => a.Gen)
                .HasForeignKey(a => a.GenId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
