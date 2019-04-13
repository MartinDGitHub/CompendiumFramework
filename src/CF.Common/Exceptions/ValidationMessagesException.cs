using System;
using System.Collections.Generic;
using System.Linq;
using CF.Common.Messaging;

namespace CF.Common.Exceptions
{
    public class ValidationMessagesException : Exception, IValidationMessagesException
    {
        public IEnumerable<IMessage> ValidationMessages { get; }

        public ValidationMessagesException(IEnumerable<IMessage> validationMessages) : base()
        {
            this.ValidationMessages = validationMessages ?? throw new ArgumentNullException(nameof(validationMessages));

            if (!this.ValidationMessages.Any())
            {
                throw new ArgumentException("No user validation messages were supplied.");
            }
        }
    }
}
