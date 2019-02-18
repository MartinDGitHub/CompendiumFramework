using System;
using System.Collections.Generic;
using System.Text;

namespace CF.Common.Messaging
{
    public interface IMessage
    {
        DateTimeOffset Timestamp { get; }
        MessageSeverity Severity { get; }
        string Value { get; }
    }
}
