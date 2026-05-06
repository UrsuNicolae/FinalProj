using Tekwill.Library.Domain.Entities;

namespace Tekwill.Library.Application.Interfaces
{
    public interface IChatRepository
    {
        Task CreateChat(Chat chat, CancellationToken ct = default);

        Task<IEnumerable<ChatNotification>> GetAllChatsForNewBookNotification(CancellationToken ct);

        Task<Chat?> GetChat(long id, CancellationToken ct = default);
    }
}
