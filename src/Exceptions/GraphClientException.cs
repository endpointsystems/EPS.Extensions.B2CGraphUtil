
using System;

namespace EPS.Extensions.B2CGraphUtil.Exceptions
{
    /// <summary>
    /// An exception caught by the graph client.
    /// </summary>
    public class GraphClientException: ApplicationException
    {
        /// <summary>
        /// Creates a new exception.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="code">The error code.</param>
        public GraphClientException(string message, int code) : base(message)
        {
            Code = code;
        }
        /// <summary>
        /// The error code.
        /// </summary>
        public int Code { get; set; }

    }
}
