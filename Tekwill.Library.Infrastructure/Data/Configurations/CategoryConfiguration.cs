using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tekwill.Library.Domain.Entities;

namespace Tekwill.Library.Infrastructure.Data.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("categories");
            builder.HasKey(b => b.Id);
            builder.Property(b => b.Id)
                .ValueGeneratedOnAdd();
            builder.Property(b => b.Name)
                .HasMaxLength(50)
                .IsRequired();
            builder.HasMany(b => b.Books)
                .WithOne(b => b.Category)
                .HasForeignKey(b => b.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
