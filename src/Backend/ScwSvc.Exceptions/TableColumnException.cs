namespace ScwSvc.Exceptions;

public class TableColumnException : InvalidTableException
{
    public TableColumnException() : base()
    {
    }

    public TableColumnException(string message) : base(message)
    {
    }

    public TableColumnException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
