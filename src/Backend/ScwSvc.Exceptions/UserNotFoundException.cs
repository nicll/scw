using System;

namespace ScwSvc.Exceptions;

public class UserNotFoundException : InvalidUserException
{
    public Guid UserId { get; init; }

    public string UserName { get; init; }

    public UserNotFoundException() : base()
    {
    }

    public UserNotFoundException(string message) : base(message)
    {
    }

    public UserNotFoundException(string message, System.Exception innerException) : base(message, innerException)
    {
    }
}
