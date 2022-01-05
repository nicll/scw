namespace ScwSvc.Exceptions;

public class TableNotFoundException : InvalidTableException
{
    public Guid TableId { get; init; }

    public TableNotFoundException() : base()
    {
    }

    public TableNotFoundException(string message) : base(message)
    {
    }

    public TableNotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
