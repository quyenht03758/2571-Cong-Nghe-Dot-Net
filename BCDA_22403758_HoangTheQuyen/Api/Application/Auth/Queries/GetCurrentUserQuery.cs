// Application/Auth/Queries/GetCurrentUserQuery.cs
using K8sManager.Api.Application.Auth.Commands;
using K8sManager.Api.Domain.Repositories;

namespace K8sManager.Api.Application.Auth.Queries;

public record GetCurrentUserQuery(int UserId);

public class GetCurrentUserQueryHandler
{
    private readonly IUserRepository _userRepo;

    public GetCurrentUserQueryHandler(IUserRepository userRepo)
    {
        _userRepo = userRepo;
    }

    public async Task<UserInfo?> HandleAsync(GetCurrentUserQuery query)
    {
        var user = await _userRepo.FindByIdAsync(query.UserId).ConfigureAwait(false);
        if (user == null) return null;

        return new UserInfo(
            user.Id,
            user.Username,
            user.Email?.Value ?? "",
            user.FullName,
            user.Role.ToString(),
            user.CreatedAt,
            user.IsActive
        );
    }
}
