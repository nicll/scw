using System;

namespace ScwSvc
{
    /// <summary>
    /// This exception is thrown when an invalid table is encountered.
    /// </summary>
    public class InvalidTableException : Exception
    {
        public InvalidTableException() : base()
        {
        }

        public InvalidTableException(string message) : base(message)
        {
        }

        public InvalidTableException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
