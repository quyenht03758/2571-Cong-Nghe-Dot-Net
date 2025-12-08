// Api/Domain/Exceptions/DomainException.cs
namespace K8sManager.Api.Domain.Exceptions;

/// <summary>
/// Base exception for all domain-level exceptions
/// </summary>
public abstract class DomainException : Exception
{
    protected DomainException(string message) : base(message)
    {
    }

    protected DomainException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public DomainException()
    {
    }
}

/// <summary>
/// Thrown when password validation fails
/// </summary>
public class InvalidPasswordException : DomainException
{
    public InvalidPasswordException(string message) : base(message)
    {
    }

    public InvalidPasswordException()
    {
    }
}

/// <summary>
/// Thrown when trying to create user with existing username
/// </summary>
public class DuplicateUsernameException : DomainException
{
    public string Username { get; }

    public DuplicateUsernameException(string username)
        : base($"Username '{username}' is already taken")
    {
        Username = username;
    }

    public DuplicateUsernameException()
    {
    }
}

/// <summary>
/// Thrown when trying to access locked user account
/// </summary>
public class UserLockedException : DomainException
{
    public int UserId { get; }

    public UserLockedException(int userId)
        : base($"User account (ID: {userId}) is locked")
    {
        UserId = userId;
    }

    public UserLockedException()
    {
    }
}

/// <summary>
/// Thrown when session token is invalid or expired
/// </summary>
public class InvalidSessionTokenException : DomainException
{
    public InvalidSessionTokenException(string message) : base(message)
    {
    }

    public InvalidSessionTokenException()
    {
    }
}

/// <summary>
/// Thrown when email validation fails
/// </summary>
public class InvalidEmailException : DomainException
{
    public InvalidEmailException(string message) : base(message)
    {
    }

    public InvalidEmailException()
    {
    }
}

/// <summary>
/// Thrown when username validation fails
/// </summary>
public class InvalidUsernameException : DomainException
{
    public InvalidUsernameException(string message) : base(message)
    {
    }

    public InvalidUsernameException()
    {
    }
}

/// <summary>
/// Thrown for general domain validation errors
/// </summary>
public class DomainValidationException : DomainException
{
    public DomainValidationException(string message) : base(message)
    {
    }

    public DomainValidationException()
    {
    }
}

