using K8sManager.Api.Domain.Entities;
using K8sManager.Api.Domain.Repositories;

namespace K8sManager.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAppUserRepository _users;
        public AuthService(IAppUserRepository users) => _users = users;

        public async Task<int?> ValidateLoginAsync(string username, string password)
        {
            var user = await _users.FindByUsernameAsync(username).ConfigureAwait(false);
            if (user == null) return null;
            return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash) ? user.Id : null;
        }

        public async Task EnsureAdminAsync()
        {
            var admin = await _users.FindByUsernameAsync("admin").ConfigureAwait(false);
            if (admin == null)
            {
                await _users.CreateAsync(new AppUser
                {
                    Username = "admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                    Role = "Admin",
                    DisplayName = "Administrator"
                }).ConfigureAwait(false);
            }
        }
    }
}