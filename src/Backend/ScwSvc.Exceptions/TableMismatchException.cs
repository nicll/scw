namespace ScwSvc.Exceptions;

public class TableMismatchException : InvalidTableException
{
    public TableMismatchException() : base()
    {
    }

    public TableMismatchException(string message) : base(message)
    {
    }

    public TableMismatchException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
