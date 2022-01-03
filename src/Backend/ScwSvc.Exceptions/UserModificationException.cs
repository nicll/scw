using System;
using ScwSvc.Models;

namespace ScwSvc.Exceptions;

/// <summary>
/// This exception is thrown when an invalid change made was to a <see cref="User"/>.
/// </summary>
public class UserModificationException : InvalidUserException
{
    /// <summary>
    /// Contains the <see cref="User.UserId"/>.
    /// </summary>
    public Guid UserId { get; init; }

    /// <summary>
    /// The previously stored value.
    /// </summary>
    public object OldValue { get; init; }

    /// <summary>
    /// The new invalid value.
    /// </summary>
    public object NewValue { get; init; }

    public UserModificationException() : base()
    {
    }

    public UserModificationException(string message) : base(message)
    {
    }

    public UserModificationException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
