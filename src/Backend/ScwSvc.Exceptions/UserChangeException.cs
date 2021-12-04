using System;
using ScwSvc.Models;

namespace ScwSvc.Exceptions
{
    /// <summary>
    /// This exception is thrown when an invalid change made was to a <see cref="User"/>.
    /// </summary>
    public class UserChangeException : Exception
    {
        /// <summary>
        /// Contains the <see cref="User.UserId"/>.
        /// </summary>
        public Guid UserId { get; init; }

        /// <summary>
        /// The previously stored value.
        /// </summary>
        public object OldValue { get; init; }

        /// <summary>
        /// The new invalid value.
        /// </summary>
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
