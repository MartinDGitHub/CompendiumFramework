using System;
using System.Collections.Generic;
using System.Text;

namespace CF.Common.Messaging
{
    public interface IScopedMessageRecorder
    {
        IEnumerable<IMessage> Messages { get; }

        void Record(MessageSeverity severity, string message);
    }
}
