namespace Tekwill.Library.Domain.Entities
{
    public class ChatNotification
    {
        public long NotificationId { get; set; }
        public Notification? Notification{ get; set; }

        public long ChatId { get; set; }
        public Chat? Chat { get; set; }
    }
}
