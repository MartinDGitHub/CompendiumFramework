using System.Collections.Generic;
using CF.Common.Messaging;

namespace CF.Common.Exceptions
{
    public interface IConsumerFriendlyMessagesException
    {
        IEnumerable<IMessage> ConsumerFriendlyMessages { get; }
    }
}