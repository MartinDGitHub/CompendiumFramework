using CF.Common.Dto.Messaging;
using System.Collections.Generic;
using System.Xml.Linq;

namespace CF.Common.Dto.ActionResults
{
    public class ActionResultPackage<TResult> : ActionResultPackage
    {
        /// <summary>
        /// Gets and sets the result of an operation.
        /// </summary>
        public TResult Result { get; set; }
    }

    public class ActionResultPackage
    {
        /// <summary>
        /// Gets and sets whether an operation succeeded.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Gets and sets consumer-friendly messages.
        /// </summary><remarks>
        /// These messages contain information that an end user can either act on, or learn from.
        /// </remarks>
        public IEnumerable<Message> Messages { get; set; }

        /// <summary>
        /// Get a consumer-friendly message for when an unexpected error occurred.
        /// </summary>
        public Message UnexpectedErrorMessage { get; set; }

        /// <summary>
        /// Gets and sets the string representation of an unhandled exception for debugging purposes only.
        /// </summary>
        public string ExceptionString { get; set; }

        /// <summary>
        /// Gets and sets a correlation ID which associate sets of log entries and provide a way to
        /// look up log entries based on IDs shown to end users/consumers.
        /// </summary>
        public string CorrelationId { get; set; }

    }
}
