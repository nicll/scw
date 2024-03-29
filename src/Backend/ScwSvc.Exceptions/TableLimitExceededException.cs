﻿namespace ScwSvc.Exceptions;

public class TableLimitExceededException : InvalidTableException
{
    public TableLimitExceededException() : base()
    {
    }

    public TableLimitExceededException(string message) : base(message)
    {
    }

    public TableLimitExceededException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
