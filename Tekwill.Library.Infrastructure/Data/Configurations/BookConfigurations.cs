using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tekwill.Library.Domain.Entities;

namespace Tekwill.Library.Infrastructure.Data.Configurations
{
    public class BookConfigurations : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            builder.ToTable("books");
            builder.HasKey(b => b.Id);
            builder.Property(b => b.Id)
                .ValueGeneratedOnAdd();
            builder.Property(b => b.Title)
                .HasMaxLength(50)
                .IsRequired();
            builder.Property(b => b.ISBN)
                .HasMaxLength(50)
                .IsRequired();

            builder.HasOne(b => b.Author)
                .WithMany(a => a.Books)
                .HasForeignKey(a => a.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(b => b.Category)
                .WithMany(a => a.Books)
                .HasForeignKey(a => a.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
