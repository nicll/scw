namespace ScwSvc.Exceptions;

public class UserCredentialsInvalidException : InvalidUserException
{
    public string InvalidValue { get; init; }

    public UserCredentialsInvalidException() : base()
    {
    }

    public UserCredentialsInvalidException(string message) : base(message)
    {
    }

    public UserCredentialsInvalidException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
