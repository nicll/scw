﻿namespace ScwSvc.Exceptions;

public class TableCollaboratorException : InvalidTableException
{
    public TableCollaboratorException() : base()
    {
    }

    public TableCollaboratorException(string message) : base(message)
    {
    }

    public TableCollaboratorException(string message, System.Exception innerException) : base(message, innerException)
    {
    }
}
