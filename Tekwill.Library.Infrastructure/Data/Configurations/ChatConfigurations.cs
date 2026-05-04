using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tekwill.Library.Domain.Entities;

namespace Tekwill.Library.Infrastructure.Data.Configurations
{
    public class ChatConfigurations : IEntityTypeConfiguration<Chat>
    {
        public void Configure(EntityTypeBuilder<Chat> builder)
        {
            builder.ToTable("chats");
            builder.HasKey(b => b.Id);
            builder.Property(b => b.Id)
                .ValueGeneratedNever();
            builder.Property(b => b.FirstName)
                .HasMaxLength(300);
            builder.Property(b => b.LastName)
                .HasMaxLength(300);
            builder.Property(b => b.UserName)
                .HasMaxLength(300);
            builder.HasMany(b => b.ChatNotifications)
                .WithOne(b => b.Chat)
                .HasForeignKey(b => b.ChatId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
