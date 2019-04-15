using CF.Common.Dto.Messaging;
using System.Collections.Generic;

namespace CF.Common.Dto.ActionResults
{
    public class ActionResultPackage<TData>
    {
        /// <summary>
        /// Gets the result data for the package.
        /// </summary>
        public TData Data { get; set; }

        /// <summary>
        /// Gets whether the action succeeded.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Gets consumer-friendly validation messages.
        /// </summary>
        public IEnumerable<Message> ValidationMessages { get; set; }

        /// <summary>
        /// Get a consumer-friendly message for when an unexpected error occurred.
        /// </summary>
        public IEnumerable<Message> UnexpectedErrorMessage { get; set; }

        /// <summary>
        /// Gets the correlation ID for associating log messages to an action result, etc.
        /// </summary>
        public string CorrelationId { get; set; }
    }
}
