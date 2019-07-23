using CF.Common.Correlation;
using CF.Common.Dto.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CF.Common.Messaging
{
    internal class ScopedMessageRecorder : IScopedMessageRecorder
    {
        private IScopedCorrelationIdProvider _scopedCorrelationIdProvider { get; }

        private IList<IMessage> _messages = new List<IMessage>();
        public IEnumerable<IMessage> Messages => this._messages;

        public bool HasErrors => this._messages.Any(x => x.Severity == MessageSeverity.Error);

        public ScopedMessageRecorder(IScopedCorrelationIdProvider scopedCorrelationIdProvider)
        {
            this._scopedCorrelationIdProvider = scopedCorrelationIdProvider ?? throw new ArgumentNullException(nameof(scopedCorrelationIdProvider));
        }

        public void Clear()
        {
            this._messages.Clear();
        }

        public void Record(MessageSeverity severity, string text)
        {
            this._messages.Add(new Message { CorrelationId = this._scopedCorrelationIdProvider.CorrelationId, Severity = severity, Text = text });
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
