using System;

namespace CF.Common.Messaging
{
    public interface IRedirectMessage: IMessage
    {
        DateTimeOffset Timestamp { get; }

        string CorrelationId { get; }

        MessageSeverity Severity { get; }

        string Text { get; }
    }
}
