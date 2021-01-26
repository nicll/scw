using System;

namespace ScwSvc
{
    public class UserChangeException : Exception
    {
        public Guid UserId { get; init; }

        public object OldValue { get; init; }

        public object NewValue { get; init; }

        public UserChangeException() : base()
        {
        }

        public UserChangeException(string message) : base(message)
        {
        }

        public UserChangeException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
