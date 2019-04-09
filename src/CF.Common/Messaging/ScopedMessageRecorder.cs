using CF.Common.Dto.Messages;
using System;
using System.Collections.Generic;

namespace CF.Common.Messaging
{
    class ScopedMessageRecorder : IScopedMessageRecorder
    {
        // Use a list to preserve recording order.
        private List<IMessage> _messages = new List<IMessage>();
        public IEnumerable<IMessage> Messages => this._messages;

        public void Record(MessageSeverity severity, string text)
        {
            this._messages.Add(new Message { Severity = severity, Text = text });
        }

        public void Record(IMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            this._messages.Add(message);
        }
    }
}
