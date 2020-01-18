using CF.Common.Correlation;
using System;
using System.Collections.Generic;

namespace CF.Common.Messaging
{
    internal class ScopedRedirectMessageRecorder : IScopedRedirectMessageRecorder
    {
        private readonly IScopedMessageRecorder _inboundMessageRecorder;

        private readonly IScopedMessageRecorder _outboundMessageRecorder;

        public Uri RefererUri { get; set; }

        public IEnumerable<IMessage> InboundMessages => this._inboundMessageRecorder.Messages;

        public IEnumerable<IMessage> OutboundMessages => this._outboundMessageRecorder.Messages;

        public ScopedRedirectMessageRecorder(IScopedCorrelationIdProvider scopedCorrelationIdProvider)
        {
            if (scopedCorrelationIdProvider == null)
            {
                throw new ArgumentNullException(nameof(scopedCorrelationIdProvider));
            }

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
    }
}
