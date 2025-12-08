// Api/Infrastructure/Security/BcryptPasswordHasher.cs
using K8sManager.Api.Domain.Services;

namespace K8sManager.Api.Infrastructure.Security;

/// <summary>
/// Implementation of IPasswordHasher using BCrypt
/// </summary>
public class BcryptPasswordHasher : IPasswordHasher
{
    private const int WorkFactor = 12; // BCrypt work factor (higher = more secure but slower)

    public string HashPassword(string plainPassword)
    {
        if (string.IsNullOrWhiteSpace(plainPassword))
            throw new ArgumentException("Password cannot be empty", nameof(plainPassword));

        return BCrypt.Net.BCrypt.HashPassword(plainPassword, WorkFactor);
    }

    public bool VerifyPassword(string plainPassword, string hashedPassword)
    {
        if (string.IsNullOrWhiteSpace(plainPassword))
            return false;

        if (string.IsNullOrWhiteSpace(hashedPassword))
            return false;

        try
        {
            return BCrypt.Net.BCrypt.Verify(plainPassword, hashedPassword);
        }
        catch
        {
            // Invalid hash format or other BCrypt errors
            return false;
        }
    }
}
