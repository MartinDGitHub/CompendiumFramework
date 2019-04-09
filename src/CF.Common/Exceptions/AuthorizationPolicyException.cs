using CF.Common.Authorization.Policies;
using CF.Common.Dto.Messages;
using CF.Common.Messaging;
using System;
using System.Collections.Generic;

namespace CF.Common.Exceptions
{
    public class AuthorizationPolicyException : Exception, IConsumerFriendlyMessagesException, ICorrelatedException 
    {
        public IEnumerable<IMessage> ConsumerFriendlyMessages { get; }

        public Guid? CorrelationGuid { get; }

        public IEnumerable<string> UnauthorizedReasons { get; }

        public AuthorizationPolicyException(PolicyResult policyResult) : this(policyResult, null)
        {
        }

        public AuthorizationPolicyException(PolicyResult policyResult, Guid? correlationGuid) : base(GetErrorMessage(policyResult, correlationGuid))
        {
            if (policyResult == null)
            {
                throw new ArgumentNullException(nameof(policyResult));
            }

            this.UnauthorizedReasons = new List<string>(policyResult.UnauthorizedReasons ?? new string[] { });

            this.ConsumerFriendlyMessages = new IMessage[]
            {
                new Message { Severity = MessageSeverity.Error, Text = policyResult.ConsumerFriendlyMessage ?? string.Empty }
            };

            this.CorrelationGuid = correlationGuid;
        }

        private static string GetErrorMessage(PolicyResult policyResult, Guid? correlationGuid)
        {
            return $"Message [{policyResult.ConsumerFriendlyMessage}]\nCorrelation GUID [{correlationGuid}]\nPolicy [{policyResult.Policy.GetType().FullName}] did not authorize for the following reasons:\n[\n[{string.Join("],\n[", policyResult.UnauthorizedReasons)}]\n]";
        }
    }
}
