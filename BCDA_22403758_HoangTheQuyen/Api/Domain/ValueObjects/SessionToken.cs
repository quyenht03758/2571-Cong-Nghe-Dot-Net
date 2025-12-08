// Api/Domain/ValueObjects/SessionToken.cs
using K8sManager.Api.Domain.Exceptions;

namespace K8sManager.Api.Domain.ValueObjects;

/// <summary>
/// Value Object for Session Token
/// </summary>
public sealed class SessionToken : IEquatable<SessionToken>
{
    public string Value { get; }
    public DateTime ExpiresAt { get; }
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;

    private SessionToken(string value, DateTime expiresAt)
    {
        Value = value;
        ExpiresAt = expiresAt;
    }

    /// <summary>
    /// Generate a new session token
    /// </summary>
    public static SessionToken Generate(TimeSpan? expiresIn = null)
    {
        var token = Guid.NewGuid().ToString("N");
        var expiry = DateTime.UtcNow.Add(expiresIn ?? TimeSpan.FromHours(8));

        return new SessionToken(token, expiry);
    }

    /// <summary>
    /// Create from existing token value
    /// </summary>
    public static SessionToken FromExisting(string token, DateTime expiresAt)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new InvalidSessionTokenException("Session token cannot be empty");

        return new SessionToken(token, expiresAt);
    }

    /// <summary>
    /// Validate if token is still valid
    /// </summary>
    public void ValidateNotExpired()
    {
        if (IsExpired)
            throw new InvalidSessionTokenException("Session token has expired");
    }

    public bool Equals(SessionToken? other)
    {
        if (other is null) return false;
        return Value.Equals(other.Value, StringComparison.Ordinal);
    }

    public override bool Equals(object? obj) => obj is SessionToken token && Equals(token);
    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => Value;

    public static implicit operator string(SessionToken token) => token.Value;
}

