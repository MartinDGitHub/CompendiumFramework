using System;
using System.Collections.Generic;
using System.Linq;
using CF.Common.Messaging;

namespace CF.Common.Exceptions
{
    public class ValidationFailedException : Exception, IConsumerFriendlyMessagesException
    {
        public IEnumerable<IMessage> ConsumerFriendlyMessages { get; }

        public ValidationFailedException(IEnumerable<IMessage> validationMessages) : base()
        {
            this.ConsumerFriendlyMessages = validationMessages ?? throw new ArgumentNullException(nameof(validationMessages));

            if (!this.ConsumerFriendlyMessages.Any())
            {
                throw new ArgumentException("No user validation messages were supplied.");
            }
        }
    }
}
