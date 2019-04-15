using CF.Common.Dto.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CF.Common.Messaging
{
    class ScopedMessageRecorder : IScopedMessageRecorder
    {
        // Use a list to preserve recording order.
        private List<IMessage> _messages = new List<IMessage>();
        public IEnumerable<IMessage> Messages => this._messages;

        public bool HasErrors => this._messages.Any(x => x.Severity == MessageSeverity.Error);
            
        public void Clear()
        {
            this._messages.Clear();
        }

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
