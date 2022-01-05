namespace ScwSvc.Exceptions;

public class UserAlreadyExistsException : InvalidUserException
{
    public UserAlreadyExistsException() : base()
    {
    }

    public UserAlreadyExistsException(string message) : base(message)
    {
    }

    public UserAlreadyExistsException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
