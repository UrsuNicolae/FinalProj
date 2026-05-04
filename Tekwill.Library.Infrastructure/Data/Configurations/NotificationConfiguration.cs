using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Tekwill.Library.Domain.Entities;
using Tekwill.Library.Domain.Enums;

namespace Tekwill.Library.Infrastructure.Data.Configurations
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.ToTable("notifications");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Type)
                .HasConversion(new EnumToNumberConverter<NotificationType, int>());
            builder.HasMany(c => c.ChatNotifications)
                .WithOne(c => c.Notification)
                .HasForeignKey(c => c.NotificationId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
