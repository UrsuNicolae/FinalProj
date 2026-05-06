using Microsoft.EntityFrameworkCore;
using Tekwill.Library.Application.Interfaces;
using Tekwill.Library.Domain.Entities;
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
