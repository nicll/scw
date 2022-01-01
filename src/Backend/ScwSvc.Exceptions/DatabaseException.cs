using System;

namespace ScwSvc.Exceptions;

public class DatabaseException : Exception
{
    public DatabaseException() : base()
    {
    }

    public DatabaseException(string? message) : base(message)
    {
    }

    public DatabaseException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
