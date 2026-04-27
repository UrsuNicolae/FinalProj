using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Tekwill.Library.Application.Interfaces;
using Tekwill.Library.Domain.Entities;
using Tekwill.Library.Infrastructure.Data;

namespace Tekwill.Library.Infrastructure.Persistance
{
    public class UserRepository : IUserRepository
    {
        private readonly LibraryContext context;

        public UserRepository(LibraryContext context)
        {
            this.context = context;
        }
        public async Task<User> CreateUser(User user, CancellationToken ct = default)
        {
            var userFromDb = await GetUserByEmail(user.Email, ct);
            if(userFromDb != null)
            {
                throw new ArgumentException("Email already used!");
            }
            await context.Users.AddAsync(user, ct);
            await context.SaveChangesAsync(ct);
            return user;
        }

        public async Task<User?> GetUserByEmail(string email, CancellationToken ct = default)
        {
            return await context.Users.FirstOrDefaultAsync(u => u.Email == email, ct);
        }
    }
}
