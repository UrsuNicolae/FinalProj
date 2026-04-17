using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tekwill.Library.Domain.Entities;

namespace Tekwill.Library.Infrastructure.Data.Configurations
{
    public class AuthorConfigurations : IEntityTypeConfiguration<Author>
    {
        public void Configure(EntityTypeBuilder<Author> builder)
        {
            builder.ToTable("authors");
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Id)
                .ValueGeneratedOnAdd();
            builder.Property(a => a.FirstName)
                .HasMaxLength(50)
                .IsRequired();
            builder.Property(a => a.LastName)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(a => a.BirthDate)
                .IsRequired();

            builder.Property(a => a.Site)
                .HasMaxLength(250)
                .IsRequired();

            builder.HasMany(a => a.Books)
                .WithOne(b => b.Author)
                .HasForeignKey(b => b.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(a => a.AuthorGens)
                .WithOne(g => g.Author)
                .HasForeignKey(g => g.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
