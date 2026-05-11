using Tekwill.Library.Domain.Entities;
using Tekwill.Library.Domain.Enums;

namespace Tekwill.Library.Application.Interfaces
{
    public interface IChatRepository
    {
        Task CreateChat(Chat chat, CancellationToken ct = default);

        Task<IEnumerable<ChatNotification>> GetAllChatsForNewBookNotification(CancellationToken ct);

        Task<Chat?> GetChat(long id, CancellationToken ct = default);

        Task CreateChatNotification(long chatId, NotificationType type, CancellationToken ct);
        Task DeleteChatNotification(long chatId, NotificationType type, CancellationToken ct);
    }
}
