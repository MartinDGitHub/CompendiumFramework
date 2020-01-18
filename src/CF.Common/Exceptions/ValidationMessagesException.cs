using System;
using System.Collections.Generic;
using System.Linq;
using CF.Common.Dto.Messaging;
using CF.Common.Messaging;

namespace CF.Common.Exceptions
{
    public class ValidationMessagesException : Exception, IValidationMessagesException
    {
        public IEnumerable<IMessage> ValidationMessages { get; }

        public ValidationMessagesException()
        {
        }

        public ValidationMessagesException(string message) : base(message)
        {
            this.ValidationMessages = new IMessage[] { new Message() { Text = message } };
        }

        public ValidationMessagesException(string message, Exception innerException) : base(message, innerException)
        {
            this.ValidationMessages = new IMessage[] { new Message() { Text = message } };
        }
        
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
