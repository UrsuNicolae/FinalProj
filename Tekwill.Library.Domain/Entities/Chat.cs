namespace Tekwill.Library.Domain.Entities
{
    public class Chat
    {
        public long Id { get; set; }

        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsForm { get; set; }
        public string Type { get; set; }
        public ICollection<ChatNotification>? ChatNotifications { get; set; }

    }
}
