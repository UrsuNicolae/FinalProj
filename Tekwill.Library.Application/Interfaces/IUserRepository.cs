using Tekwill.Library.Domain.Entities;

namespace Tekwill.Library.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<User> CreateUser(User user, CancellationToken ct = default);

        Task<User?> GetUserByEmail(string email, CancellationToken ct = default);
    }
}
