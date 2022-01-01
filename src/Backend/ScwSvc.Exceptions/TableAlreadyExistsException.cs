using System;

namespace ScwSvc.Exceptions;

public class TableAlreadyExistsException : InvalidTableException
{
    public Guid TableId { get; init; }

    public TableAlreadyExistsException() : base()
    {
    }

    public TableAlreadyExistsException(string message) : base(message)
    {
    }

    public TableAlreadyExistsException(string message, System.Exception innerException) : base(message, innerException)
    {
    }
}
