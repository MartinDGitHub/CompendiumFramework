using CF.Common.Correlation;
using System;
using System.Collections.Generic;

namespace CF.Common.Messaging
{
    internal class ScopedRedirectMessageRecorder : IScopedRedirectMessageRecorder
    {
        private IScopedCorrelationIdProvider _scopedCorrelationIdProvider { get; }

        private IScopedMessageRecorder _inboundMessageRecorder;

        private IScopedMessageRecorder _outboundMessageRecorder;

        public string ReferrerUrl { get; set; }

        public IEnumerable<IMessage> InboundMessages => this._inboundMessageRecorder.Messages;

        public IEnumerable<IMessage> OutboundMessages => this._outboundMessageRecorder.Messages;

        public bool HasErrors => throw new NotImplementedException();

        public ScopedRedirectMessageRecorder(IScopedCorrelationIdProvider scopedCorrelationIdProvider)
        {
            this._scopedCorrelationIdProvider = scopedCorrelationIdProvider ?? throw new ArgumentNullException(nameof(scopedCorrelationIdProvider));
            this._inboundMessageRecorder = new ScopedMessageRecorder(scopedCorrelationIdProvider);
            this._outboundMessageRecorder = new ScopedMessageRecorder(scopedCorrelationIdProvider);
        }

        public void ClearInbound()
        {
            this._inboundMessageRecorder.Clear();
        }

        public void ClearOutbound()
        {
            this._outboundMessageRecorder.Clear();
        }

        public void RecordInbound(IMessage message)
        {
            this._inboundMessageRecorder.Record(message);
        }

        public void RecordOutbound(IMessage message)
        {
            this._outboundMessageRecorder.Record(message);
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public void Record(MessageSeverity severity, string text)
        {
            throw new NotImplementedException();
        }

        public void Record(IMessage message)
        {
            throw new NotImplementedException();
        }
    }
}
