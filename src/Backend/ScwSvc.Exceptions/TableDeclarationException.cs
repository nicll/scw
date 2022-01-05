namespace ScwSvc.Exceptions;

public class TableDeclarationException : InvalidTableException
{
    public TableDeclarationException() : base()
    {
    }

    public TableDeclarationException(string message) : base(message)
    {
    }

    public TableDeclarationException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
