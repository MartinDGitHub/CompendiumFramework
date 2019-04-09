using CF.Common.Messaging;
using System;

namespace CF.Common.Dto.Messages
{
    public class Message : IMessage
    {
        public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.Now;

        public MessageSeverity Severity { get; set; }

        public string Text { get; set; }
    }
}
