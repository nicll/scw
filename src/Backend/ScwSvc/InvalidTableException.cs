using System;

namespace ScwSvc
{
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
