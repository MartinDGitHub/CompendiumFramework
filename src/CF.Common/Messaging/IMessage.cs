using System;

namespace CF.Common.Messaging
{
    public interface IMessage
    {
        /// <summary>
        /// Gets the correlation ID for the message.
        /// </summary>
        string CorrelationId { get; }

        /// <summary>
        /// Gets the timestamp of when the message was recorded in UTC.
        /// </summary>
        DateTimeOffset Timestamp { get; }

        /// <summary>
        /// Gets the severity of the message.
        /// </summary>
        MessageSeverity Severity { get; }

        /// <summary>
        /// Gets the content of the message.
        /// </summary>
        string Text { get; }
    }
}
