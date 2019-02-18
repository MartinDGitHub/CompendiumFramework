using System;
using System.Collections.Generic;
using System.Text;

namespace CF.Common.Messaging
{
    class ScopedMessageRecorder : IScopedMessageRecorder
    {
        private class Message : IMessage
        {
            public DateTimeOffset Timestamp { get; private set; }

            public MessageSeverity Severity { get; private set; }

            public string Value { get; private set; }

            public Message(MessageSeverity severity, string value)
            {
                this.Timestamp = DateTimeOffset.Now;
                this.Severity = severity;
                this.Value = value;
            }
        }

        private HashSet<IMessage> _messages = new HashSet<IMessage>();
        public IEnumerable<IMessage> Messages => this._messages;

        public void Record(MessageSeverity severity, string value)
        {
            this._messages.Add(new Message(severity, value));
        }
    }
}
