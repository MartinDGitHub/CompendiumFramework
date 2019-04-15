using CF.Common.Messaging;
using System;

namespace CF.Common.Dto.Messaging
{
    public class Message : IMessage
    {
        public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.Now;

        public MessageSeverity Severity { get; set; }

        public string Text { get; set; }
    }
}
