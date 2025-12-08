// Api/Domain/ValueObjects/Email.cs
using System.Text.RegularExpressions;
using K8sManager.Api.Domain.Exceptions;

namespace K8sManager.Api.Domain.ValueObjects;

/// <summary>
/// Value Object for Email with validation
/// </summary>
public sealed class Email : IEquatable<Email>
{
    private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public string Value { get; }

    private Email(string value)
    {
        Value = value;
    }

    public static Email Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new InvalidEmailException("Email cannot be empty");

        email = email.Trim().ToLowerInvariant();

        if (!EmailRegex.IsMatch(email))
            throw new InvalidEmailException($"Invalid email format: {email}");

        if (email.Length > 200)
            throw new InvalidEmailException("Email cannot exceed 200 characters");

        return new Email(email);
    }

    public static Email? CreateOptional(string? email)
    {
        return string.IsNullOrWhiteSpace(email) ? null : Create(email);
    }

    public bool Equals(Email? other)
    {
        if (other is null) return false;
        return Value.Equals(other.Value, StringComparison.OrdinalIgnoreCase);
    }

    public override bool Equals(object? obj) => obj is Email email && Equals(email);
    public override int GetHashCode() => Value.GetHashCode(StringComparison.OrdinalIgnoreCase);
    public override string ToString() => Value;

    public static bool operator ==(Email? left, Email? right) =>
        left?.Equals(right) ?? right is null;

    public static bool operator !=(Email? left, Email? right) => !(left == right);

    public static implicit operator string(Email email) => email.Value;
}

