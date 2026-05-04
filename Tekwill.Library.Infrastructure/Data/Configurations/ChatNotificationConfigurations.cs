using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tekwill.Library.Domain.Entities;

namespace Tekwill.Library.Infrastructure.Data.Configurations
{
    public class ChatNotificationConfigurations : IEntityTypeConfiguration<ChatNotification>
    {
        public void Configure(EntityTypeBuilder<ChatNotification> builder)
        {
            builder.ToTable("chat_notifications");
            builder.HasKey(c => new { c.ChatId, c.NotificationId });
            builder.HasOne(c => c.Notification)
                .WithMany(c => c.ChatNotifications)
                .HasForeignKey(c => c.NotificationId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(c => c.Chat)
                .WithMany(c => c.ChatNotifications)
                .HasForeignKey(c => c.ChatId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
