using Microsoft.EntityFrameworkCore;
using Tekwill.Library.Application.Interfaces;
using Tekwill.Library.Domain.Entities;
using Tekwill.Library.Domain.Enums;
using Tekwill.Library.Infrastructure.Data;

namespace Tekwill.Library.Infrastructure.Persistance
{
    public class ChatRepository : IChatRepository
    {
        private readonly LibraryContext context;

        public ChatRepository(LibraryContext context)
        {
            this.context = context;
        }
        public async Task CreateChat(Chat chat, CancellationToken ct = default)
        {
            await context.Chats.AddAsync(chat, ct);
            await context.SaveChangesAsync(ct);
        }

        public async Task CreateChatNotification(long chatId, NotificationType type, CancellationToken ct)
        {
            var chatFromDb = await context.Chats.FirstOrDefaultAsync(c => c.Id == chatId, ct);
            if (chatFromDb == null)
            {
                throw new KeyNotFoundException("Chat id not found!");
            }
            var notification = await context.Notifications.FirstOrDefaultAsync(n => n.Type == type, ct);
            if (notification == null)
            {
                throw new KeyNotFoundException("Notification not found!");
            }

            var chatNotificationFromDb = await context.ChatNotifications.FirstOrDefaultAsync(c => c.ChatId == chatId && c.NotificationId == notification.Id, ct);
            if (chatNotificationFromDb != null)
            {
                return;
            }
            var chatNotificationToAdd = new ChatNotification
            {
                ChatId = chatId,
                NotificationId = notification.Id
            };
            await context.ChatNotifications.AddAsync(chatNotificationToAdd, ct);
            await context.SaveChangesAsync(ct);
        }

        public async Task DeleteChatNotification(long chatId, NotificationType type, CancellationToken ct)
        {
            var notification = await context.Notifications.FirstOrDefaultAsync(n => n.Type == type, ct);
            if (notification == null)
            {
                throw new KeyNotFoundException("Notification not found!");
            }

            var chatNotificationToDelete = await context.ChatNotifications.FirstOrDefaultAsync(c => c.ChatId == chatId && c.NotificationId == notification.Id, ct);
            if (chatNotificationToDelete == null)
            {
                throw new KeyNotFoundException("ChatNotification not found!");
            }
            context.ChatNotifications.Remove(chatNotificationToDelete);
            await context.SaveChangesAsync(ct);
        }

        public async Task<IEnumerable<ChatNotification>> GetAllChatsForNewBookNotification(CancellationToken ct)
        {
            return await context
                .ChatNotifications
                .AsNoTracking()
                .Include(cn => cn.Notification)
                .Where(n => n.Notification.Type == Domain.Enums.NotificationType.NewBookNotification)
                .ToListAsync(ct);
        }

        public async Task<Chat?> GetChat(long id, CancellationToken ct = default)
        {
            return await context.Chats.FirstOrDefaultAsync(c => c.Id == id, ct);
        }
    }
}
