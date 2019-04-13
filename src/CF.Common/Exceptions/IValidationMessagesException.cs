using System.Collections.Generic;
using CF.Common.Messaging;

namespace CF.Common.Exceptions
{
    public interface IValidationMessagesException
    {
        IEnumerable<IMessage> ValidationMessages { get; }
    }
}