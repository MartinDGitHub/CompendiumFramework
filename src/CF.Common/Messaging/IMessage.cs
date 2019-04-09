using CF.Common.Dto.Messages;
using System;

namespace CF.Common.Messaging
{
    public interface IMessage
    {
        DateTimeOffset Timestamp { get; }

        MessageSeverity Severity { get; }

        string Text { get; }
    }
}
