// Api/Domain/Services/ITokenGenerator.cs
namespace K8sManager.Api.Domain.Services;

/// <summary>
/// Domain service for JWT token generation and validation
/// </summary>
public interface ITokenGenerator
{
    /// <summary>
    /// Generate JWT token for authenticated user
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="username">Username</param>
    /// <param name="role">User role</param>
    /// <param name="expiresIn">Token expiration time (default: 8 hours)</param>
    /// <returns>JWT token string</returns>
    string GenerateToken(int userId, string username, string role, TimeSpan? expiresIn = null);

    /// <summary>
    /// Validate JWT token and extract claims
    /// </summary>
    /// <param name="token">JWT token</param>
    /// <returns>Dictionary of claims if valid, null if invalid</returns>
    Dictionary<string, string>? ValidateToken(string token);

    /// <summary>
    /// Extract user ID from token without full validation
    /// </summary>
    int? GetUserIdFromToken(string token);

    // Alias method for backward compatibility
    string Generate(int userId, string username, string role, TimeSpan? expiresIn = null) => GenerateToken(userId, username, role, expiresIn);
}

