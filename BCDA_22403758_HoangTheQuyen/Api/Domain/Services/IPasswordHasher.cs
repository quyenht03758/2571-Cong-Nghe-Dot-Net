// Api/Domain/Services/IPasswordHasher.cs
namespace K8sManager.Api.Domain.Services;

/// <summary>
/// Domain service for password hashing and verification
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Hash a plain text password
    /// </summary>
    string HashPassword(string plainPassword);

    /// <summary>
    /// Verify if plain password matches the hashed password
    /// </summary>
    bool VerifyPassword(string plainPassword, string hashedPassword);

    // Alias methods for backward compatibility
    string Hash(string plainPassword) => HashPassword(plainPassword);
    bool Verify(string plainPassword, string hashedPassword) => VerifyPassword(plainPassword, hashedPassword);
}

