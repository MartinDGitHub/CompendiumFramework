using CF.Common.Messaging;
using System;
using System.Globalization;

namespace CF.Common.Dto.Messaging
{
    public class Message : IMessage
    {
        public string CorrelationId { get; set; }

        public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.UtcNow;

        public MessageSeverity Severity { get; set; }

        public string Text { get; set; }

        /// <summary>
        /// Default constructor for serialization, etc.
        /// </summary>
        public Message()
        {
        }

        public Message(IMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            this.Timestamp = message.Timestamp;
            this.Severity = message.Severity;
            this.Text = message.Text;
        }

        public override string ToString()
        {
            return $"[[{this.CorrelationId}][{this.Timestamp.ToString("o", CultureInfo.InvariantCulture)}][{this.Severity}][{this.Text}]]";
        }
    }
}
