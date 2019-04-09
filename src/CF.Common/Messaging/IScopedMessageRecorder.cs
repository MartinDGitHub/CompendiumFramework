using CF.Common.Dto.Messages;
using System.Collections.Generic;

namespace CF.Common.Messaging
{
    public interface IScopedMessageRecorder
    {
        IEnumerable<IMessage> Messages { get; }

        void Record(MessageSeverity severity, string text);

        void Record(IMessage message);
    }
}
