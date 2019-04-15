using System.Collections.Generic;

namespace CF.Common.Messaging
{
    public interface IScopedMessageRecorder
    {
        IEnumerable<IMessage> Messages { get; }

        bool HasErrors { get; }

        void Clear();

        void Record(MessageSeverity severity, string text);

        void Record(IMessage message);
    }
}
