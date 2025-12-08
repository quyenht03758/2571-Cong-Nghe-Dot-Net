// Application/Sessions/SessionHandlers.cs
using K8sManager.Api.Domain.Entities;
using K8sManager.Api.Domain.Repositories;

namespace K8sManager.Api.Application.Sessions;

// Commands
public record CreateSessionCommand(int UserId, string SessionToken, DateTime ExpiresAt);
public record DeleteSessionCommand(string SessionToken);
public record CleanupExpiredSessionsCommand();

// Queries
public record GetActiveSessionsQuery(int? UserId = null);
public record GetMySessionsQuery(int UserId);

// Handlers
public class CreateSessionCommandHandler
{
    private readonly IUserSessionRepository _sessionRepo;

    public CreateSessionCommandHandler(IUserSessionRepository sessionRepo)
    {
        _sessionRepo = sessionRepo;
    }

    public async Task<int> HandleAsync(CreateSessionCommand command)
    {
        var session = new UserSession
        {
            UserId = command.UserId,
            SessionToken = command.SessionToken,
            ExpiresAt = command.ExpiresAt,
            CreatedAt = DateTime.UtcNow
        };
        var id = await _sessionRepo.CreateAsync(session).ConfigureAwait(false);
        return (int)id;
    }
}

public class DeleteSessionCommandHandler
{
    private readonly IUserSessionRepository _sessionRepo;

    public DeleteSessionCommandHandler(IUserSessionRepository sessionRepo)
    {
        _sessionRepo = sessionRepo;
    }

    public async Task<bool> HandleAsync(DeleteSessionCommand command)
    {
        return await _sessionRepo.DeleteByTokenAsync(command.SessionToken).ConfigureAwait(false);
    }
}

public class GetActiveSessionsQueryHandler
{
    private readonly IUserSessionRepository _sessionRepo;

    public GetActiveSessionsQueryHandler(IUserSessionRepository sessionRepo)
    {
        _sessionRepo = sessionRepo;
    }

    public async Task<List<UserSession>> HandleAsync(GetActiveSessionsQuery query)
    {
        var sessions = await _sessionRepo.GetActiveSessionsAsync(query.UserId).ConfigureAwait(false);
        return sessions.ToList();
    }
}

public class GetMySessionsQueryHandler
{
    private readonly IUserSessionRepository _sessionRepo;

    public GetMySessionsQueryHandler(IUserSessionRepository sessionRepo)
    {
        _sessionRepo = sessionRepo;
    }

    public async Task<List<UserSession>> HandleAsync(GetMySessionsQuery query)
    {
        var sessions = await _sessionRepo.GetByUserIdAsync(query.UserId).ConfigureAwait(false);
        return sessions.ToList();
    }
}

public class CleanupExpiredSessionsHandler
{
    private readonly IUserSessionRepository _sessionRepo;

    public CleanupExpiredSessionsHandler(IUserSessionRepository sessionRepo)
    {
        _sessionRepo = sessionRepo;
    }

    public async Task<int> HandleAsync(CleanupExpiredSessionsCommand command)
    {
        await _sessionRepo.DeleteExpiredAsync().ConfigureAwait(false);
        return 0; // Return 0 as we don't track count
    }
}
