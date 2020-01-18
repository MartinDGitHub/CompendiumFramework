using System;
using System.Collections.Generic;

namespace CF.Common.Messaging
{
    public interface IScopedRedirectMessageRecorder
    {
        /// <summary>
        /// Gets and sets the URL inbound messages originated from. This will be null
        /// for requests without a referer header.
        /// </summary>
        Uri RefererUri { get; set; }

        /// <summary>
        /// Gets messages inbound for the current request.
        /// </summary>
        IEnumerable<IMessage> InboundMessages { get; }

        /// <summary>
        /// Gets messages outbound for the redirect target request.
        /// </summary>
        IEnumerable<IMessage> OutboundMessages { get; }

        void ClearInbound();

        void ClearOutbound();

        void RecordInbound(IMessage message);

        void RecordOutbound(IMessage message);
    }
}
