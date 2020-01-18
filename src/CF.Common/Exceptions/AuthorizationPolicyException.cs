using CF.Common.Authorization.Policies;
using CF.Common.Dto.Messaging;
using CF.Common.Messaging;
using System;
using System.Collections.Generic;

namespace CF.Common.Exceptions
{
    public class AuthorizationPolicyException : Exception, IValidationMessagesException, ICorrelatedException 
    {
        public IEnumerable<IMessage> ValidationMessages { get; }

        public string CorrelationId { get; }

        public IEnumerable<string> UnauthorizedReasons { get; }

        public AuthorizationPolicyException()
        {
        }

        public AuthorizationPolicyException(string message) : base(message)
        {
        }

        public AuthorizationPolicyException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public AuthorizationPolicyException(PolicyResult policyResult) : this(policyResult, null)
        {
        }

        public AuthorizationPolicyException(PolicyResult policyResult, string correlationId) : base(GetErrorMessage(policyResult))
        {
            if (policyResult == null)
            {
                throw new ArgumentNullException(nameof(policyResult));
            }

            this.UnauthorizedReasons = new List<string>(policyResult.UnauthorizedReasons ?? Array.Empty<string>());

            this.ValidationMessages = new IMessage[]
            {
                new Message { Severity = MessageSeverity.Error, Text = policyResult.ConsumerFriendlyMessage ?? string.Empty }
            };

            this.CorrelationId = correlationId;
        }

        private static string GetErrorMessage(PolicyResult policyResult)
        {
            return $"Message [{policyResult.ConsumerFriendlyMessage}]\nPolicy [{policyResult.Policy.GetType().FullName}] did not authorize for the following reasons:\n[\n[{string.Join("],\n[", policyResult.UnauthorizedReasons)}]\n]";
        }
    }
}
