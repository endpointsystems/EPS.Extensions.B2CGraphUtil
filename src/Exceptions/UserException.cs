using System;
using Microsoft.Graph;
using Microsoft.Graph.Models;

namespace EPS.Extensions.B2CGraphUtil.Exceptions
{
    /// <summary>
    /// <see cref="ServiceException"/> wrapper for user-specific exceptions.
    /// </summary>
    public class UserException: ApplicationException
    {
        /// <summary>
        /// Create a new exception instance with the <see cref="User"/> in contention.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="user">The <see cref="User"/>.</param>
        /// <param name="inner">The inner <see cref="ServiceException"/></param>
        public UserException(string message, User user, Exception inner):
            base(message,inner)
        {
            User = user;
        }

        /// <summary>
        /// Create a new exception instance with the <see cref="User"/> in contention.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="inner">The inner <see cref="ServiceException"/></param>
        public UserException(string message, Exception inner): base(message,inner)
        {
            User = null;
        }

        /// <summary>
        /// The <see cref="User"/> object that is having the exception.
        /// </summary>
        public User User { get; set; }
    }
}
