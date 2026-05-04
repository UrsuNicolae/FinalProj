using Tekwill.Library.Domain.Enums;

namespace Tekwill.Library.Domain.Entities
{
    public class Notification
    {
        public long Id { get; set; }

        public NotificationType Type { get; set; }

        public ICollection<ChatNotification>? ChatNotifications { get; set; }
    }


}
