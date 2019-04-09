using CF.Common.Dto.Messages;
using System;
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
        /// Gets consumer-friendly messages.
        /// </summary>
        public IEnumerable<Message> ConsumerFriendlyMessages { get; set; }

        /// <summary>
        /// Gets the correlation GUID for associating log messages, etc.
        /// </summary>
        public Guid CorrelationGuid { get; set; }
    }
}
