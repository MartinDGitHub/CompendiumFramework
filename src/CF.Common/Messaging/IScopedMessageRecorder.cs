using System;
using System.Collections.Generic;
using System.Text;

namespace CF.Common.Messaging
{
    public interface IScopedMessageRecorder
    {
        void Record(MessageSeverity severity, string message);
    }
}
