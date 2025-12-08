// Api/Domain/ValueObjects/Password.cs
using K8sManager.Api.Domain.Exceptions;

namespace K8sManager.Api.Domain.ValueObjects;

/// <summary>
/// Value Object for Password with validation rules
/// </summary>
public sealed class Password
{
    private const int MinLength = 8;
    private const int MaxLength = 100;

    public string Value { get; }

    private Password(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Create and validate a plain text password
    /// </summary>
    public static Password Create(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new InvalidPasswordException("Password cannot be empty");

        if (password.Length < MinLength)
            throw new InvalidPasswordException($"Password must be at least {MinLength} characters long");

        if (password.Length > MaxLength)
            throw new InvalidPasswordException($"Password cannot exceed {MaxLength} characters");

        // Basic strength validation
        bool hasUpper = password.Any(char.IsUpper);
        bool hasLower = password.Any(char.IsLower);
        bool hasDigit = password.Any(char.IsDigit);

        if (!hasUpper || !hasLower || !hasDigit)
            throw new InvalidPasswordException(
                "Password must contain at least one uppercase letter, one lowercase letter, and one digit");

        return new Password(password);
    }

    /// <summary>
    /// Create password without validation (for already hashed passwords)
    /// </summary>
    public static Password CreateWithoutValidation(string hashedPassword)
    {
        if (string.IsNullOrWhiteSpace(hashedPassword))
            throw new ArgumentException("Hashed password cannot be empty");

        return new Password(hashedPassword);
    }

    public static implicit operator string(Password password) => password.Value;
    public override string ToString() => "***"; // Never expose password value
}

